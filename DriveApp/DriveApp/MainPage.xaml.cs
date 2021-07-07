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
        DatabaseManager databaseManager = new DatabaseManager();
        Tracking tracking = new Tracking();
        private void Button_Clicked(object sender, EventArgs e)
        {
            tracking.isTracking = !tracking.isTracking;
            if (tracking.isTracking)
            {
                Button.BackgroundColor = Color.FromHex("0fbd21");
                Button.Text = "Aktiv";
                Thread getLocation = new Thread(UpdatePosition);
                Thread getSpeed = new Thread(GetSpeed);
                getLocation.Start();
                getSpeed.Start();
            }
            else
            {
                Button.BackgroundColor = Color.FromHex("cc2118");
                Button.Text = "Slukket";
                if (tracking.distance == 0)
                {
                    return;
                }
                else
                {
                    databaseManager.WriteToDataBase(tracking.distance);
                    tracking.distance = 0;

                }
            }

        }
        async Task RecieveSpeed()
        {
            int speed = await tracking.RecieveSpeed();
            try
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Speed.Text = $"Hastighed: {speed}";
                });
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }

        }
        async Task RecieveLocation()
        {
            tracking.distance = await tracking.RecieveLocation();
            Device.BeginInvokeOnMainThread(() =>
            {
                GPS.Text = tracking.ToString();
            });
        }

        public async void UpdatePosition()
        {
            while (tracking.isTracking)
            {
                await RecieveLocation();
                Thread.Sleep(1000);
            }
        }
        public async void GetSpeed()
        {
            while (tracking.isTracking)
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
            if (tracking.distance == 0)
            {
                await Navigation.PushAsync(new MyProfilePage());

            }
            else
            {
                if (tracking.isTracking)
                {
                    Button.BackgroundColor = Color.FromHex("cc2118");
                    Button.Text = "Slukket";
                    tracking.isTracking = false;
                    databaseManager.WriteToDataBase(tracking.distance);
                    tracking.distance = 0;
                    await Navigation.PushAsync(new MyProfilePage());

                }
                else
                {
                    await Navigation.PushAsync(new MyProfilePage());

                }
            }

        }
    }

}
