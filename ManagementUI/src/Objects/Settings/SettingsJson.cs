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
        private string _path;

        #region PROPERTIES
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("editor")]
        public SettingsLauncher Editor { get; set; }
        [JsonProperty("settings")]
        public Settings Settings { get; set; }

        #endregion

        #region CONSTRUCTORS
        public SettingsJson() : base() { }
        public SettingsJson(string filePath) : base(filePath) { }

        #endregion

        #region PUBLIC METHODS

        public static SettingsJson ReadFromFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("Path cannot be null.");

            if (!File.Exists(path))
            {
                CreateAppFolders();
                var job = new JObject
                {
                    JValue.CreateComment("Reserved for later use."),
                    new JProperty("version", "1.0"),
                    JValue.CreateComment("Possible enumeration values are: [ \"notepad\", \"notepadplusplus\", \"vscode\" ]"),
                    new JProperty("editor", "notepad"),
                    new JProperty("settings", new JObject
                    {
                        new JProperty("icons", new JArray())
                    })
                };
                File.WriteAllText(path, job.ToString());
            }
            string text = File.ReadAllText(path);
            var resolver = new JsonSerializerSettings();
            resolver.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
            SettingsJson sj = JsonConvert.DeserializeObject<SettingsJson>(text, resolver);
            sj._path = path;
            return sj;
        }
        private void Settings_Changed(object sender, NotifyCollectionChangedEventArgs e) => this.Save();
        //public void WriteSettings()
        //{
        //    var resolver = new JsonSerializerSettings
        //    {
        //        Formatting = Formatting.Indented,
        //        NullValueHandling = NullValueHandling.Include,
        //        DefaultValueHandling = DefaultValueHandling.Include
        //    };
        //    resolver.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
        //    string jsonStr = JsonConvert.SerializeObject(this, resolver);
        //    File.WriteAllText(_path, jsonStr);
        //}

        public void Save(SettingChangedAction action = SettingChangedAction.Save)
        {
            using (StreamWriter streamWriter = new StreamWriter(this.FilePath))
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
                    ((IJsonSettings)this).SettingsAsJson.WriteTo(writer, new StringEnumConverter(new CamelCaseNamingStrategy()));
                }
            }

            base.OnSettingsChanged(new SettingsChangedEventArgs(action));
        }

        #endregion

        #region BACKEND/PRIVATE METHODS
        private static void CreateAppFolders()
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

        [OnDeserialized]
        private void OnDeserialized(StreamingContext ctx)
        {
            if (this.Settings != null && this.Settings.Apps != null)
            {
                this.Settings.Apps.CollectionChanged += this.Settings_Changed;
            }
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