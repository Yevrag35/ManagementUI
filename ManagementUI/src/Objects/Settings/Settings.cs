using MG.Settings.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ManagementUI
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class Settings
    {
        #region PROPERTIES
        [JsonProperty("icons")]
        public AppSettingCollection Apps { get; set; }

        #endregion

        #region CONSTRUCTORS
        public Settings() => Apps = new AppSettingCollection();

        #endregion

        #region PUBLIC METHODS
        public AppIconSetting SettingFromIcon(AppListItem ali)
        {
            return this.Apps.Find(x => x.Name.Equals(ali.AppName) && x.IconPath.Equals(ali.Path));
        }

        #endregion
    }
}