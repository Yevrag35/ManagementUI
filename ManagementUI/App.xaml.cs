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
        internal static IntPtr MyHandle { get; set; }
        internal static SettingsJson Settings { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Settings = SettingsJson.ReadFromFile(Environment.GetEnvironmentVariable("LOCALAPPDATA") + "\\Mike Garvey\\ManagementUI\\settings.json");
            var main = new MUI();
            main.Show();
        }
    }
}
