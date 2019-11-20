using MG.Settings.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;

namespace ManagementUI
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class SettingsJson : JsonSettingsManager
    {
        

        #region PROPERTIES
        [JsonProperty("$schema")]
        private const string SCHEMA = "https://json.yevrag35.com/uimanagement/v1.0.0/schema.json";

        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("editor")]
        public SettingsLauncher Editor { get; set; }
        [JsonProperty("settings")]
        public Settings Settings { get; set; }

        #endregion

        #region CONSTRUCTORS
        public SettingsJson() : base() { }
        public SettingsJson(string filePath) : base(filePath)
        {
            if (!File.Exists(filePath))
            {
                this.CreateAppFolders();
                this.SaveDefault();
            }
        }

        #endregion

        #region PUBLIC METHODS
        //private void Settings_Changed(object sender, NotifyCollectionChangedEventArgs e) => this.Save();

        public void Save(SettingChangedAction action = SettingChangedAction.Save)
        {
            this.Save(((IJsonSettings)this).SettingsAsJson, action);
        }

        #endregion

        #region DEFAULTS
        //private JsonSerializerSettings GetDefaultSerializerSettings()
        //{
        //    var settings = new JsonSerializerSettings
        //    {
        //        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        //        DateFormatHandling = DateFormatHandling.IsoDateFormat,
        //        DateTimeZoneHandling = DateTimeZoneHandling.Local,
        //        DefaultValueHandling = DefaultValueHandling.Include,
        //        FloatParseHandling = FloatParseHandling.Decimal,
        //        Formatting = Formatting.Indented,
        //        NullValueHandling = NullValueHandling.Include,
        //        MissingMemberHandling = MissingMemberHandling.Ignore
        //    };
        //    settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
        //    return settings;
        //}

        private JObject GetDefaultSettings()
        {
            return new JObject
            {
                new JProperty("$schema", SCHEMA),
                new JProperty("version", "1.0"),
                new JProperty("editor", SettingsLauncher.Notepad.ToString().ToLower()),
                new JProperty("settings", new JObject
                {
                    new JProperty("apps", new JArray())
                })
            };
        }

        public void SaveDefault()
        {
            JObject defSets = this.GetDefaultSettings();
            this.Save(defSets, SettingChangedAction.Save);
        }

        #endregion

        #region BACKEND/PRIVATE METHODS
        private void CreateAppFolders()
        {
            string dir1 = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            try
            {
                string dir2 = dir1 + "\\Mike Garvey";
                string dir3 = dir2 + "\\ManagementUI";
                Directory.CreateDirectory(dir1);
                Directory.CreateDirectory(dir2);
                Directory.CreateDirectory(dir3);
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format(
                    "An error occurred: {0}{1}{1}{2}{1}{1}{3}",
                    e.GetType().FullName,
                    Environment.NewLine,
                    e.Message,
                    string.Format("LOCALAPPDATA: {0}", dir1)),
                    "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //[OnDeserialized]
        //private void OnDeserialized(StreamingContext ctx)
        //{
        //    if (this.Settings != null && this.Settings.Apps != null)
        //    {
        //        this.Settings.Apps.CollectionChanged += this.Settings_Changed;
        //    }
        //}

        private void Save(JObject saveThis, SettingChangedAction action)
        {
            using (var streamWriter = new StreamWriter(this.FilePath))
            {
                using (var writer = new JsonTextWriter(streamWriter)
                {
                    AutoCompleteOnClose = true,
                    CloseOutput = true,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    Formatting = Formatting.Indented,
                    IndentChar = char.Parse("\t"),
                    Indentation = 1
                })
                {
                    saveThis.WriteTo(writer, new StringEnumConverter(new CamelCaseNamingStrategy()));
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }

            base.OnSettingsChanged(new SettingsChangedEventArgs(action));
        }

        #endregion
    }

    public enum SettingsLauncher
    {
        Notepad,
        NotepadPlusPlus,
        VsCode
    }
}