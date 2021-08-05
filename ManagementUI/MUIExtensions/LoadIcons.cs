using Newtonsoft.Json;
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

        //private string GetSettings()
        //{
        //    string temp = Environment.GetEnvironmentVariable("LOCALAPPDATA");
        //    return string.Format(RELATIVE_PATH, temp);
        //}

        //private void LoadIcons(SettingsJson settings, out AppListCollection outList)
        //{
        //    outList = new AppListCollection(settings.Settings.Apps.Count);
        //    for (int i = 0; i < settings.Settings.Apps.Count; i++)
        //    {
        //        AppIconSetting app = settings.Settings.Apps[i];
        //        AppListItem ali = app.ToListItem(App.MyHandle);
        //        if (!string.IsNullOrEmpty(app.Arguments))
        //        {
        //            ali.Arguments = app.Arguments;
        //        }
        //        outList.Add(ali);
        //    }
        //    this.AppListView.ItemsSource = outList.View;
        //    this.AppListView.Items.Refresh();
        //}

        //private AppListCollection LoadApps(SettingsJson jsonSettings)
        //{
        //    var appList = new AppListCollection(jsonSettings.Settings.Apps.Count);
        //    for (int i = 0; i < )
        //}
    }
}
