using System;
using System.Collections.Generic;
using System.Globalization;
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
            CultureInfo cultureInfo = new CultureInfo("da-DK");
            Calendar calendar = cultureInfo.Calendar;
            CalendarWeekRule weekRule = cultureInfo.DateTimeFormat.CalendarWeekRule;
            DayOfWeek dayOfWeek = cultureInfo.DateTimeFormat.FirstDayOfWeek;

            List <MyStats> stats = await database.GetStats();
            double todaysDistance = 0;
            double WeekDistance = 0;
            double MonthDistance = 0;
            for (int i = 0; i < stats.Count; i++)
            {
                if (stats[i].Timecreated == DateTime.Today)
                {
                    
                    todaysDistance += stats[i].Distance;
                   
                }
                if (calendar.GetWeekOfYear(stats[i].Timecreated, weekRule, dayOfWeek) == calendar.GetWeekOfYear(DateTime.Today, weekRule, dayOfWeek) && stats[i].Timecreated.Year == DateTime.Today.Year)
                {
                    if (stats[i].Timecreated == DateTime.Today)
                    {

                    }
                    WeekDistance += stats[i].Distance;
                    WeekStats.Text += $"{stats[i].Day}: Kørt: {stats[i].Distance}\n";
                }
                if (stats[i].Timecreated.Month == DateTime.Today.Month)
                {
                    if (stats[i].Timecreated == DateTime.Today)
                    {

                    }
                    MonthDistance += stats[i].Distance;
                    MonthStats.Text += $"{stats[i].Day}: Kørt: {stats[i].Distance}\n";
                }
            }
            KmToday.Text = $"Kørt idag {Math.Truncate(todaysDistance * 1000) / 1000}";
            KmWeek.Text = $"Kørt denne uge {Math.Truncate(WeekDistance * 1000) / 1000}";
            KmMonth.Text = $"Kørt denne måned {Math.Truncate(MonthDistance * 1000) / 1000}";
        }
    }
}