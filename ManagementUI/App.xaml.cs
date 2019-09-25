using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ManagementUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static List<AppListItem> _items;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppListItem.PerformIconCleanup(Environment.GetEnvironmentVariable("TEMP"));
            _items = new List<AppListItem>();
            var main = new MUI();
            main.Show();
        }
    }
}
