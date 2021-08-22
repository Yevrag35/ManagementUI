using MG.Settings.Json;
using MG.Settings.Json.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using ManagementUI.Json;

using Strings = ManagementUI.Properties.Resources;


namespace ManagementUI
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class SettingsJson : JsonSettings, IDisposable
    {
        [JsonIgnore]
        private bool _disposed;
        [JsonIgnore]
        private EditorManager _editorManager;

        #region PROPERTIES
        [JsonProperty("$schema", Order = 1)]
        public string Schema { get; private set; }

        [JsonProperty("autoValidateCredentials")]
        public bool AutoValidate { get; set; } = true;

        [JsonProperty("customEditors", Order = 4)]
        public EditorManager EditorManager
        {
            get => _editorManager;
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException(nameof(this.EditorManager));

                else if (null != _editorManager)
                    _editorManager.Dispose();

                _editorManager = value;
            }
        }

        [JsonProperty("version", Order = 2)]
        public string Version { get; set; }
        [JsonProperty("editor", Order = 5)]
        public string Editor { get; set; }

        #endregion

        #region CONSTRUCTORS
        public SettingsJson()
            : base(GetFolderPath(), GetFileName(), GetEncoding(), GetDefaultSettings(), true, GetSerializer())
        {
        }

        #endregion

        #region PUBLIC METHODS
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                this.EditorManager?.Dispose();
                _disposed = true;
            }
        }

        #endregion

        #region DEFAULTS

        private static JObject GetDefaultSettings()
        {
            return new JObject()
            {
                GetDefault(x => x.Schema, Strings.Settings_SchemaUrl),
                GetDefault(x => x.Version, Strings.Settings_Version),
                GetDefault(x => x.AutoValidate, false),
                GetDefault(x => x.Editor, Strings.Settings_DefaultEditor),
                GetDefault(x => x.EditorManager, new JArray())
            };
        }

        private static JProperty GetDefault<TProp>(Expression<Func<SettingsJson, TProp>> expression, object defaultValue)
        {
            string name = GetJsonName(expression);
            return new JProperty(name, defaultValue);
        }

        #endregion

        #region BACKEND/PRIVATE METHODS
        internal static Encoding GetEncoding() => Encoding.UTF8;
        private static string GetFileName()
        {
#if DEBUG
            return Strings.SettingsFileName_Debug;
#else
            return Strings.SettingsFileName;
#endif
        }
        internal static string GetFolderPath()
        {
            return Environment.ExpandEnvironmentVariables(Strings.SettingsPath);
        }
        internal static string GetFullPath()
        {
            return Path.Combine(GetFolderPath(), GetFileName());
        }
        private static string GetJsonName<TProp>(Expression<Func<SettingsJson, TProp>> expression)
        {
            MemberInfo memInfo = GetPropertyInfo(expression);

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

        internal static MemberInfo GetPropertyInfo<TClass, TProp>(Expression<Func<TClass, TProp>> expression)
            where TClass : JsonSettings
        {
            MemberInfo info = null;
            if (expression.Body is MemberExpression memEx)
                info = memEx.Member;

            else if (expression.Body is UnaryExpression unEx && unEx.Operand is MemberExpression unExMem)
                info = unExMem.Member;

            return info;
        }

        internal static JsonSerializer GetSerializer()
        {
            var serializer = new JsonSerializer
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateParseHandling = DateParseHandling.DateTime,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                FloatParseHandling = FloatParseHandling.Double,
                MissingMemberHandling = MissingMemberHandling.Error,
                NullValueHandling = NullValueHandling.Include
            };
            serializer.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
            return serializer;
        }

        #endregion
    }
}