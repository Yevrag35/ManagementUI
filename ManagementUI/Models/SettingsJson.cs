﻿using MG.Settings.Json;
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
    public class SettingsJson : JsonSettings
    {
        #region PROPERTIES
        [JsonProperty("$schema")]
        private string _schema;

        [JsonProperty("customEditors")]
        public EditorManager EditorManager { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("editor")]
        public string Editor { get; set; }

        #endregion

        #region CONSTRUCTORS
        public SettingsJson()
            : base(GetFolderPath(), GetFileName(), GetEncoding(), GetDefaultSettings(), true, GetSerializer())
        {
        }

        #endregion

        #region PUBLIC METHODS
        //private void Settings_Changed(object sender, NotifyCollectionChangedEventArgs e) => this.Save();

        //public void Save(SettingChangedAction action = SettingChangedAction.Save)
        //{
        //    this.Save(((IJsonSettings)this).SettingsAsJson, action);
        //}

        #endregion

        #region DEFAULTS

        private static JObject GetDefaultSettings()
        {
            return new JObject()
            {
                GetDefault(x => x._schema, Strings.Settings_SchemaUrl),
                GetDefault(x => x.Version, Strings.Settings_Version),
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
        //private void CreateAppFolders()
        //{
        //    string dir1 = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        //    try
        //    {
        //        string dir2 = dir1 + "\\Mike Garvey";
        //        string dir3 = dir2 + "\\ManagementUI";
        //        Directory.CreateDirectory(dir1);
        //        Directory.CreateDirectory(dir2);
        //        Directory.CreateDirectory(dir3);
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(string.Format(
        //            "An error occurred: {0}{1}{1}{2}{1}{1}{3}",
        //            e.GetType().FullName,
        //            Environment.NewLine,
        //            e.Message,
        //            string.Format("LOCALAPPDATA: {0}", dir1)),
        //            "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        internal static Encoding GetEncoding() => Encoding.UTF8;

        internal static string GetFolderPath()
        {
            return Environment.ExpandEnvironmentVariables(Strings.SettingsPath);
        }

        private static string GetFileName()
        {
#if DEBUG
            return Strings.SettingsFileName_Debug;
#else
            return Strings.SettingsFileName;
#endif
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

        //[OnDeserialized]
        //private void OnDeserialized(StreamingContext ctx)
        //{
        //    if (this.Settings != null && this.Settings.Apps != null)
        //    {
        //        this.Settings.Apps.CollectionChanged += this.Settings_Changed;
        //    }
        //}

        //private void Save(JObject saveThis, SettingChangedAction action)
        //{
        //    using (var streamWriter = new StreamWriter(this.FilePath))
        //    {
        //        using (var writer = new JsonTextWriter(streamWriter)
        //        {
        //            AutoCompleteOnClose = true,
        //            CloseOutput = true,
        //            DateFormatHandling = DateFormatHandling.IsoDateFormat,
        //            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
        //            Formatting = Formatting.Indented,
        //            IndentChar = char.Parse("\t"),
        //            Indentation = 1
        //        })
        //        {
        //            saveThis.WriteTo(writer, new StringEnumConverter(new CamelCaseNamingStrategy()));
        //            streamWriter.Flush();
        //            streamWriter.Close();
        //        }
        //    }

        //    base.OnSettingsChanged(new SettingsChangedEventArgs(action));
        //}

        #endregion
    }

    public enum SettingsLauncher
    {
        Notepad,
        NotepadPlusPlus,
        VsCode
    }
}