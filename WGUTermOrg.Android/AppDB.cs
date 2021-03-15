using System.IO;
using SQLite;
using WGUTermOrg.Droid;
using Xamarin.Forms;
using Environment = System.Environment;

[assembly: Dependency(typeof(AppDb))]

namespace WGUTermOrg.Droid
{
    public class AppDb : IAppDatabase
    {
        public SQLiteAsyncConnection GetConnection()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "AppDatabase.db3");
            return new SQLiteAsyncConnection(dbPath);
        }
    }
}