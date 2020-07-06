using ManagementUI.src.Objects.Collections;
using MG.Settings.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace ManagementUI
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class Settings
    {
        #region PROPERTIES
        [JsonProperty("apps", DefaultValueHandling = DefaultValueHandling.Populate)]
        public AppListViewCollection Apps { get; set; }

        #endregion

        #region CONSTRUCTORS
        [JsonConstructor]
        public Settings() { }

        #endregion

        #region PUBLIC METHODS
        [Obsolete]
        public AppIconSetting SettingFromIcon(AppListItem ali)
        {
            //return this.Apps.Find(x => x.Name.Equals(ali.AppName) && x.IconPath.Equals(ali.Path));
            return null;
        }

        #endregion

        [OnDeserialized]
        private void OnDeserialized(StreamingContext ctx)
        {
            if (this.Apps != null && this.Apps.Count > 0)
            {
                this.Apps.RemoveAll(x => !x.Exists);
            }
        }
    }
}