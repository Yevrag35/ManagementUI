using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;

namespace ManagementUI
{
    [Serializable]
    public class AppIconSetting
    {
        [JsonProperty("arguments")]
        public string Arguments { get; set; }
        [JsonProperty("exePath")]
        public string ExePath { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("iconPath")]
        public string Path { get; set; }
        [JsonProperty("iconIndex")]
        public int Index { get; set; }

        public ProcessStartInfo NewStartInfo()
        {
            return new ProcessStartInfo
            {
                Arguments = this.Arguments,
                CreateNoWindow = true,
                FileName = this.Path,
                UseShellExecute = false
            };
        }
        public AppListItem ToListItem() => new AppListItem(this.Name, App.MyHandle, this.Path, this.Index);
    }
}
