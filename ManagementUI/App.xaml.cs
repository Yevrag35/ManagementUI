﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

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
            string path = Environment.GetEnvironmentVariable("LOCALAPPDATA") + 
                "\\Mike Garvey\\ManagementUI\\settings.json";

            Settings = SettingsJson.ReadFromFile(path);
            var main = new MUI();
            main.Show();
            
        }
    }
}
