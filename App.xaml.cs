using PrintOrderManager.Data;
using System.Windows;

namespace PrintOrderManager
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize DB
            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();
            }
        }
    }
}
