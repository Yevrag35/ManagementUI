using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ManagementUI.Json.Preferences
{
    //[JsonConverter]
    public class PreferencesModel
    {
        public string Version { get; }


        [JsonConstructor]
        public PreferencesModel()
        {
            this.Version = this.GetVersionFromAttribute(this.GetType().Assembly?.CustomAttributes);
            if (string.IsNullOrWhiteSpace(this.Version))
            {
                this.Version = "1.0.0";
            }
        }

        private string GetVersionFromAttribute(IEnumerable<CustomAttributeData> attributes)
        {
            if (attributes != null && attributes is IReadOnlyList<CustomAttributeData> readOnlyList && readOnlyList.Count > 0)
            {
                CustomAttributeData attData = readOnlyList.FirstOrDefault(x => x.AttributeType.Equals(typeof(AssemblyFileVersionAttribute)));
                if (attData != null && attData.ConstructorArguments.Count > 0)
                {
                    return attData.ConstructorArguments[0].Value as string;
                }
            }
            return null;
        }
    }
}
