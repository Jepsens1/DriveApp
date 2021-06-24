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
                ((Button)sender).BackgroundColor = Color.Green;
                ((Button)sender).Text = "Tracking";
                Thread getLocation = new Thread(UpdatePosition);
                Thread getSpeed = new Thread(GetSpeed);
                getLocation.Start();
                getSpeed.Start();
            }
            else
            {
                databaseManager.WriteToDataBase();
                ((Button)sender).BackgroundColor = Color.Red;
                ((Button)sender).Text = "Start Tracking";
            }

        }
        async Task RecieveSpeed()
        {
            int speed = await tracking.RecieveSpeed();
            try
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Speed.Text = $"Speed: {speed}";
                });
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }

        }
        async Task RecieveLocation()
        {
            string location = await tracking.RecieveLocation();
            Device.BeginInvokeOnMainThread(() =>
            {
                GPS.Text = location;
            });
        }
        //needs some work
        
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
            databaseManager.WriteToDataBase();
            await Navigation.PushAsync(new MyProfilePage());
        }
    }

}
