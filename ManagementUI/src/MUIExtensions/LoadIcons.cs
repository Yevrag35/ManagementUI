﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementUI
{
    public partial class MUI
    {
        private const string RELATIVE_PATH = @"{0}\Mike Garvey\ManagementUI\settings.json";

        private string GetSettings()
        {
            string temp = Environment.GetEnvironmentVariable("LOCALAPPDATA");
            return string.Format(RELATIVE_PATH, temp);
        }

        private void LoadIcons(IntPtr windowHandle, SettingsJson settings, out AppList outList)
        {
            outList = new AppList(settings.Settings.Icons.Count);
            for (int i = 0; i < settings.Settings.Icons.Count; i++)
            {
                AppIconSetting app = settings.Settings.Icons[i];
                AppListItem ali = app.ToListItem(App.MyHandle);
                if (!string.IsNullOrEmpty(app.Arguments))
                {
                    ali.Arguments = app.Arguments;
                }
                outList.Add(ali);
            }
            this.AppListView.ItemsSource = outList.AppView;
            this.AppListView.Items.Refresh();
        }

    }
}