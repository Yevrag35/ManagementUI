using Newtonsoft.Json;
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
            if (!File.Exists(path))
            {
                CreateAppFolders(path);
                var job = new JObject
                {
                    new JProperty("version", "1.0"),
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
            return sj;
        }

        #endregion

        #region BACKEND/PRIVATE METHODS
        private static void CreateAppFolders(string path)
        {
            string dir1 = Environment.GetEnvironmentVariable("LOCALAPPDATA");
            string dir2 = dir1 + "\\Mike Garvey";
            string dir3 = dir2 + "\\ManagementUI";
            Directory.CreateDirectory(dir1);
            Directory.CreateDirectory(dir2);
            Directory.CreateDirectory(dir3);
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