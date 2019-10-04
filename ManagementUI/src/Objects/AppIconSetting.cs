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
        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

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
        public AppListItem ToListItem(IntPtr handle)
        {
            if (string.IsNullOrEmpty(this.Path))
                this.Path = this.ExePath;

            var ali = new AppListItem(this.Name, handle, this.Path, this.ExePath, this.Index);

            if (this.Tags != null && this.Tags.Count > 0)
                ali.Tags = this.Tags;

            return ali;
        }
    }

    public class AppIconSettingSorter : IComparer<AppIconSetting>
    {
        public int Compare(AppIconSetting x, AppIconSetting y) => x.Name.CompareTo(y.Name);
    }
}
