using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace DriveApp
{
    class MyStatsDatabase
    {
        static SQLiteAsyncConnection db;
        static async Task Init()
        {
            if (db != null)
            {
                return;
            }
            string databasePath = Path.Combine(FileSystem.AppDataDirectory, "MyStats.db");
            db = new SQLiteAsyncConnection(databasePath);
            await db.CreateTableAsync<MyStats>();
            Console.WriteLine("Table Created");
        }
        public async Task AddStats(double distance)
        {
            await Init();
            MyStats stats = new MyStats
            {
                Day = DateTime.Now.DayOfWeek.ToString(),
                Timecreated = DateTime.Today.Date,
                Distance = distance,
            };
            int id = await db.InsertAsync(stats);
        }
        public async Task RemoveStats(int id)
        {
            await Init();
            await db.DeleteAsync<MyStats>(id);
        }
        public async Task<List<MyStats>> GetStats()
        {
            await Init();
            List<MyStats> stats = await db.Table<MyStats>().ToListAsync();
            return stats;
        }
    }
}
