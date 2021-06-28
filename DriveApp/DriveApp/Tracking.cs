using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DriveApp
{
    class Tracking
    {

        CancellationTokenSource cts;
        public bool isTracking { get; set; } = false;
        public Location lastlocation { get; set; }
        public double distance { get; set; } = 0;
        public async Task<int> RecieveSpeed()
        {
            await CheckLocationPermission();
            try
            {
                if (lastlocation != null)
                {
                    int kmh = Convert.ToInt32((lastlocation.Speed * 3600) / 1000);
                    return kmh;

                }
            }

            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
            return 0;

        }
        public async Task<double> RecieveLocation()
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

                return distance;
                 //$"Latitude: {(int)currentLocation.Latitude}\nLongitude: {(int)currentLocation.Longitude}\nAltitude: {(int)currentLocation.Altitude}\nDistance: {Math.Truncate(distance * 1000) / 1000} km";

            }

            catch (FeatureNotSupportedException)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage.DisplayAlert("Not supported", "Enhed understøtter ikke denne app", "OK");
                });
                return 0;
            }
            catch (FeatureNotEnabledException)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage.DisplayAlert("GPS ikke tilsluttet", "Fejl opstod, vær venlig at sikre GPS er slået til", "OK");
                });
                return 0;
                // Handle not enabled on device exception
            }
            catch (PermissionException)
            {

                return 0;
            }
            catch (Exception)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage.DisplayAlert("Lokation ikke fundet", "Prøv igen", "OK");

                });
                return 0;
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

    }
}
