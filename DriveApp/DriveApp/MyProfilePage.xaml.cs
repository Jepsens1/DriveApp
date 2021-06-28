using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            List<MyStats> stats = await database.GetStats();
            double todayTotal = 0;
            double weekTotal = 0;
            double monthTotal = 0;
            DaysDistance[] daysDistances = new DaysDistance[7];
            for (int i = 0; i < 7; i++)
            { 
                daysDistances[i] = new DaysDistance((Days)i, 0);
            }
            for (int i = 0; i < stats.Count; i++)
            {
                if (stats[i].Timecreated == DateTime.Today)
                {
                    todayTotal += stats[i].Distance;
                }
                if (calendar.GetWeekOfYear(stats[i].Timecreated, weekRule, dayOfWeek) == calendar.GetWeekOfYear(DateTime.Today, weekRule, dayOfWeek) && stats[i].Timecreated.Year == DateTime.Today.Year)
                {
                    switch (stats[i].Timecreated.DayOfWeek)
                    {
                        case DayOfWeek.Friday:
                            daysDistances[4].Distance += stats[i].Distance;
                            break;
                        case DayOfWeek.Monday:
                            daysDistances[0].Distance += stats[i].Distance;
                            break;
                        case DayOfWeek.Saturday:
                            daysDistances[5].Distance += stats[i].Distance;
                            break;
                        case DayOfWeek.Sunday:
                            daysDistances[6].Distance += stats[i].Distance;
                            break;
                        case DayOfWeek.Thursday:
                            daysDistances[3].Distance += stats[i].Distance;
                            break;
                        case DayOfWeek.Tuesday:
                            daysDistances[1].Distance += stats[i].Distance;
                            break;
                        case DayOfWeek.Wednesday:
                            daysDistances[2].Distance += stats[i].Distance;
                            break;
                        default:
                            break;
                    }
                    weekTotal += stats[i].Distance;
                    Debug.WriteLine($"{stats[i].Timecreated}, {DateTime.Today}");
                }
                if (stats[i].Timecreated.Month == DateTime.Today.Month)
                {
                    MonthStats.Text += $"{stats[i].Timecreated.DayOfWeek}: Kørt: {stats[i].Distance}\n";
                    monthTotal += stats[i].Distance;
                }
            }
            for (int i = 0; i < daysDistances.Length; i++)
            {
                WeekStats.Text += daysDistances[i].ToString();
            }
            KmToday.Text = $"Kørt idag {Math.Truncate(todayTotal * 1000) / 1000}";
            KmWeek.Text = $"Kørt denne uge {Math.Truncate(weekTotal * 1000) / 1000}";
            KmMonth.Text = $"Kørt denne måned {Math.Truncate(monthTotal * 1000) / 1000}";
        }
    }
}