using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using ManagementUI.Collections;
using ManagementUI.Models;

using Strings = ManagementUI.Properties.Resources;

namespace ManagementUI
{
    public partial class MUI
    {
        private async Task SaveApps()
        {
            await this.JsonAppsRead.SaveAsync();
        }
        private async Task SaveSettings()
        {
            await this.Settings.SaveAsync();
        }
    }
}