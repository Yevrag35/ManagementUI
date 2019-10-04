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
    [JsonObject(MemberSerialization.OptIn)]
    public class AppIconSetting : ICloneable
    {
        private const string MMC = "MMC";
        private const string MMC_EXE = "\\mmc.exe";
        private static readonly string MMC_PATH = Environment.GetFolderPath(Environment.SpecialFolder.System) + MMC_EXE;

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

        #region METHODS
        public AppIconSetting Clone() => new AppIconSetting
        {
            Arguments = this.Arguments,
            ExePath = this.ExePath,
            Index = this.Index,
            Name = this.Name,
            Path = this.Path,
            Tags = this.Tags
        };
        object ICloneable.Clone() => this.Clone();

        public static AppIconSetting NewMmcTemplate() => new AppIconSetting
        {
            ExePath = MMC_PATH,
            Tags = new List<string>(new string[1] { MMC })
        };
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
                ali.TagList = this.Tags;

            return ali;
        }

        #endregion
    }

    
}
