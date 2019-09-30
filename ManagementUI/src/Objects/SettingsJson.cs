﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ManagementUI
{
    [Serializable]
    public class SettingsJson
    {
        #region PROPERTIES
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("editor")]
        public SettingsLauncher Editor { get; set; }
        [JsonProperty("settings")]
        public Settings Settings { get; set; }

        #endregion

        #region CONSTRUCTORS
        public SettingsJson() { }

        #endregion

        #region PUBLIC METHODS
        public static SettingsJson ReadFromFile(string path)
        {
            SettingsJson sj = null;
            if (File.Exists(path))
            {
                string text = File.ReadAllText(path);
                var resolver = new JsonSerializerSettings();
                resolver.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
                sj = JsonConvert.DeserializeObject<SettingsJson>(text, resolver);
            }
            return sj;
        }

        #endregion

        #region BACKEND/PRIVATE METHODS


        #endregion
    }

    public enum SettingsLauncher
    {
        Notepad,
        NotepadPlusPlus,
        VsCode
    }
}