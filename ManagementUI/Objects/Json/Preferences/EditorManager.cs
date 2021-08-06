using Newtonsoft.Json;
using System;
using ManagementUI.Functionality.Models;
using ManagementUI.Json.Converters;

namespace ManagementUI.Json
{
    [JsonArray]
    [JsonConverter(typeof(EditorManagerConverter))]
    public class EditorManager : EditorManagerBase
    {
        public EditorManager()
            : base()
        {
        }
    }
}
