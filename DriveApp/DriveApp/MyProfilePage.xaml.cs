using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DriveApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyProfilePage : ContentPage
    {
        public MyProfilePage()
        {
            InitializeComponent();
            GetStatsFromDatabase();
        }
        MyStatsDatabase database = new MyStatsDatabase();
        private async void NavigationButton_Click(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage());
        }
        async void GetStatsFromDatabase()
        {
            List<MyStats> stats = await database.GetStats();
            
        KmToday.Text = $"Kørt idag {Math.Truncate(stats[stats.Count - 1].Distance * 1000) / 1000}";
        }
    }
}