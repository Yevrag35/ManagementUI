using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ManagementUI
{
    [Serializable]
    public class Settings
    {
        #region FIELDS/CONSTANTS


        #endregion

        #region PROPERTIES
        [JsonProperty("icons")]
        public List<AppIconSetting> Icons { get; set; }

        #endregion

        #region CONSTRUCTORS
        public Settings() { }

        #endregion

        #region PUBLIC METHODS
        public AppIconSetting SettingFromIcon(AppListItem ali)
        {
            return this.Icons.Find(x => x.Name.Equals(ali.AppName) && x.Path.Equals(ali.Path));
        }

        #endregion

        #region BACKEND/PRIVATE METHODS


        #endregion
    }
}