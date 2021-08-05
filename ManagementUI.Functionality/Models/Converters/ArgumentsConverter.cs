using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagementUI.Functionality.Models.Converters
{
    public class ArgumentsConverter : JsonConverter<List<string>>
    {
        public override List<string> ReadJson(JsonReader reader, Type objectType, List<string> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray && null != reader.Value)
            {
                return this.ReadFromArray(JArray.FromObject(reader.Value));
            }
            else if (reader.TokenType == JsonToken.String)
            {
                return this.ReadFromString(reader.Value);
            }
            else
                return new List<string>();
        }
        public override void WriteJson(JsonWriter writer, List<string> value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            if (null == value || value.Count <= 0)
            {
                writer.WriteEndArray();
                return;
            }

            for (int i = 0; i < value.Count; i++)
            {
                string s = value[i];
                if (!string.IsNullOrWhiteSpace(s))
                    writer.WriteValue(s);
            }

            writer.WriteEndArray();
        }

        private List<string> ReadFromArray(JArray jar)
        {
            var list = new List<string>(jar.Count);
            foreach (JToken token in jar)
            {
                if (token?.Type == JTokenType.String)
                {
                    list.Add(token.ToObject<string>());
                }
            }

            return list;
        }
        private List<string> ReadFromString(object value)
        {
            var list = new List<string>(1);
            if (value is string strVal)
            {
                list.Add(strVal);
            }

            return list;
        }
    }
}
