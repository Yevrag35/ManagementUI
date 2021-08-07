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
        private JsonSerializerSettings GetSerializerSettings(params Action<JsonSerializerSettings>[] extraSettings)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            if (null != extraSettings)
            {
                for (int i = 0; i < extraSettings.Length; i++)
                {
                    extraSettings[i](settings);
                }
            }

            return settings;
        }

        private async Task ReadApps()
        {
            string rawJson = await ReadFileAsync(JsonAppsFile.GetFullPath());

            JsonSerializerSettings settings = this.GetSerializerSettings();

            await this.Dispatcher.InvokeAsync(() => 
            {
                JsonAppsFile jsonApps = JsonConvert.DeserializeObject<JsonAppsFile>(rawJson, settings);
                this.JsonAppsRead = jsonApps;
                this.AppList.CreateView();
            });
        }

        private void ReadSettings()
        {
            string rawJson = ReadFile(SettingsJson.GetFullPath());
            JsonSerializerSettings settings = this.GetSerializerSettings(
                (s) => s.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy())));

            this.Dispatcher.Invoke(() =>
            {
                this.Settings = JsonConvert.DeserializeObject<SettingsJson>(rawJson, settings);

            });
        }
        private async Task ReadSettingsAsync()
        {
            string rawJson = await ReadFileAsync(SettingsJson.GetFullPath());

            JsonSerializerSettings settings = this.GetSerializerSettings(
                (s) => s.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy())));

            await this.Dispatcher.InvokeAsync(() =>
            {
                this.Settings = JsonConvert.DeserializeObject<SettingsJson>(rawJson, settings);

            }, DispatcherPriority.Send);
        }

        private static string ReadFile(string filePath)
        {
            return File.ReadAllText(filePath, SettingsJson.GetEncoding());
        }
        private static async Task<string> ReadFileAsync(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(fs, SettingsJson.GetEncoding()))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }
    }
}