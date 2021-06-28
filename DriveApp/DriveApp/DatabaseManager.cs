using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace DriveApp
{
    class DatabaseManager
    {
        MyStatsDatabase database = new MyStatsDatabase();
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
        public async void WriteToDataBase(double distance)
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
