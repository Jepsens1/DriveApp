using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Threading;
using System.IO;
using Xamarin.Forms.PlatformConfiguration;

namespace DriveApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
        }
        CancellationTokenSource cts;
        MyStatsDatabase database = new MyStatsDatabase();
        bool isTracking = false;
        Location lastlocation = null;
        double distance = 0;
        private void Button_Clicked(object sender, EventArgs e)
        {
            isTracking = !isTracking;
            if (isTracking)
            {
                ((Button)sender).BackgroundColor = Color.Green;
                ((Button)sender).Text = "Tracking";
                Thread getLocation = new Thread(UpdatePosition);
                Thread getSpeed = new Thread(GetSpeed);
                getLocation.Start();
                getSpeed.Start();
            }
            else
            {
                WriteToDataBase();
                ((Button)sender).BackgroundColor = Color.Red;
                ((Button)sender).Text = "Start Tracking";
            }

        }
        async Task RecieveSpeed()
        {
            await CheckLocationPermission();
            try
            {
                if (lastlocation != null)
                {
                    int kmh = Convert.ToInt32((lastlocation.Speed * 3600) / 1000);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Speed.Text = $"Speed: {kmh}";
                    });

                }
            }
            catch (Exception e)
                        {

                Console.WriteLine(e.Message);
            }

        }
        async Task RecieveLocation()
        {
            await CheckLocationPermission();
            try
            {
                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Default, TimeSpan.FromSeconds(10));
                cts = new CancellationTokenSource();
                Location currentLocation = await Geolocation.GetLocationAsync(request, cts.Token);
                if (lastlocation != null)
                {
                    distance += Location.CalculateDistance(lastlocation, currentLocation, DistanceUnits.Kilometers);

                }
                lastlocation = currentLocation;

                Device.BeginInvokeOnMainThread(() =>
                {
                    GPS.Text = $"Latitude: {(int)currentLocation.Latitude}\nLongitude: {(int)currentLocation.Longitude}\nAltitude: {(int)currentLocation.Altitude}\nDistance: {Math.Truncate(distance * 1000) / 1000} km";
                });

            }

            catch (FeatureNotSupportedException)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage.DisplayAlert("Not supported", "Enhed understøtter ikke denne app", "OK");
                });
                return;
            }
            catch (FeatureNotEnabledException)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage.DisplayAlert("GPS ikke tilsluttet", "Fejl opstod, vær venlig at sikre GPS er slået til", "OK");
                });
                return;
                // Handle not enabled on device exception
            }
            catch (PermissionException)
            {

                return;
            }
            catch (Exception)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage.DisplayAlert("Lokation ikke fundet", "Prøv igen", "OK");

                });

                return;
                // Unable to get location
            }

        }
        public async Task<PermissionStatus> CheckLocationPermission()
        {

            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status == PermissionStatus.Granted)
            {
                return status;
            }
            if (status != PermissionStatus.Granted || status == PermissionStatus.Denied)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                });
            }

            return status;

        }
        //needs some work
        public async Task CheckWritePermission()
        {

            PermissionStatus read = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            PermissionStatus write = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (read == PermissionStatus.Granted && write == PermissionStatus.Granted)
            {
                await Task.CompletedTask;
            }
            if (read != PermissionStatus.Granted && write != PermissionStatus.Granted || read == PermissionStatus.Denied && write == PermissionStatus.Denied)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Permissions.RequestAsync<Permissions.StorageRead>();
                    Permissions.RequestAsync<Permissions.StorageWrite>();
                });
                await Task.CompletedTask;
            }
        }
        public async void UpdatePosition()
        {
            while (isTracking)
            {
                await RecieveLocation();
                Thread.Sleep(1000);
            }
        }
        public async void GetSpeed()
        {
            while (isTracking)
            {
                await RecieveSpeed();
                Thread.Sleep(3000);
            }
        }
        protected override void OnDisappearing()
        {
            if (cts != null && !cts.IsCancellationRequested)
                cts.Cancel();
            base.OnDisappearing();
        }

        private async void NavigateButton_Click(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyProfilePage());
        }

        public async void WriteToDataBase()
        {
            await CheckWritePermission();
            try
            {
                await database.AddStats(distance);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

}
