using Newtonsoft.Json;
using System;
using System.IO;
using ManagementUI.Functionality.Models;
using ManagementUI.Json.Converters;

using Strings = ManagementUI.Properties.Resources;

namespace ManagementUI.Json
{
    [JsonArray]
    [JsonConverter(typeof(EditorManagerConverter))]
    public class EditorManager : EditorManagerBase, IDisposable
    {
        public EditorManager()
            : base(GetFilePath())
        {
        }

        private static string GetFilePath()
        {
#if DEBUG
            string fileName = Strings.SettingsFileName_Debug;
#else
            string fileName = Strings.SettingsFileName;
#endif
            return Path.Combine(Strings.SettingsPath, fileName);
        }
    }
}
