using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementUI
{
    public partial class MUI
    {
        private const string RELATIVE_PATH = @"{0}\Mike Garvey\ManagementUI\settings.json";

        private string GetSettingsFile()
        {
            string temp = Environment.GetEnvironmentVariable("LOCALAPPDATA");
            return string.Format(RELATIVE_PATH, temp);
        }

        private void LoadIcons(IntPtr windowHandle)
        {
            
        }

    }
}
