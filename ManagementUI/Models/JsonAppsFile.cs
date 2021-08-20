using MG.Settings.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using ManagementUI.Collections;

using Strings = ManagementUI.Properties.Resources;

namespace ManagementUI.Models
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class JsonAppsFile : JsonSettings
    {
        [JsonProperty("$schema")]
        public string Schema { get; set; }

        [JsonProperty("apps", Order = 2)]
        public AppsList Apps { get; set; }

        public JsonAppsFile()
            : base(GetFolderPath(), GetFileName(), SettingsJson.GetEncoding(), GetDefaultSettings(), true, 
                  SettingsJson.GetSerializer())
        {
        }

        private static string GetFileName() => Strings.Apps_FileName;
        private static string GetFolderPath() => Environment.ExpandEnvironmentVariables(Strings.SettingsPath);
        private static JProperty GetDefault<TProp>(Expression<Func<JsonAppsFile, TProp>> expression, object defaultValue)
        {
            string name = GetJsonName(expression);
            return new JProperty(name, defaultValue);
        }
        private static JObject GetDefaultSettings()
        {
            return new JObject
            {
                GetDefault(x => x.Schema, Strings.Apps_SchemaUrl),
                GetDefault(x => x.Apps, new JArray())
            };
        }
        internal static string GetFullPath()
        {
            return Path.Combine(GetFolderPath(), GetFileName());
        }
        private static string GetJsonName<TProp>(Expression<Func<JsonAppsFile, TProp>> expression)
        {
            MemberInfo memInfo = SettingsJson.GetPropertyInfo(expression);

            JsonPropertyAttribute jsonPropertyAttribute = memInfo
                ?.GetCustomAttributes<JsonPropertyAttribute>()
                    ?.FirstOrDefault();

            if (null == jsonPropertyAttribute || string.IsNullOrWhiteSpace(jsonPropertyAttribute.PropertyName))
            {
                if (null != memInfo)
                    return memInfo.Name;

                else
                    throw new ArgumentException("The expression did not resolve into a property or field");
            }

            return jsonPropertyAttribute.PropertyName;
        }


        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (null == this.Apps)
                this.Apps = new AppsList(new List<AppItem>());

            if (string.IsNullOrWhiteSpace(this.Schema))
                this.Schema = Strings.Apps_SchemaUrl;
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            if (null == this.Apps)
                this.Apps = new AppsList(new List<AppItem>());

            if (string.IsNullOrWhiteSpace(this.Schema))
                this.Schema = Strings.Apps_SchemaUrl;
        }
    }
}
