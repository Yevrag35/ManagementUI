using Newtonsoft.Json;
using System;
using System.IO;
using ManagementUI.Functionality.Executable;

namespace ManagementUI.Functionality.Settings
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CustomEditor : EditorBase, IEditor, ILaunchable
    {

        [JsonProperty("key", Order = 1)]
        public string Key { get; set; }

        [JsonProperty("path", Order = 2)]
        public override string ExePath { get; set; }

        [JsonProperty("args", Order = 3)]
        public override string Arguments { get; set; }

        [JsonConstructor]
        public CustomEditor()
            : base()
        {
        }
    }
}
