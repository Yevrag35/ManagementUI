using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagementUI.Converters
{
    public class FilterTagConverter : JsonConverter<HashSet<string>>
    {
        public override HashSet<string> ReadJson(JsonReader reader, Type objectType, HashSet<string> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            HashSet<string> tl = new HashSet<string>();
            JToken token = JToken.ReadFrom(reader);
            if (token.Type == JTokenType.Array && token is JArray jar)
            {
                foreach (JToken item in jar)
                {
                    if (item.Type == JTokenType.String)
                    {
                        tl.Add(item.ToObject<string>());
                    }
                }
            }

            return tl;
        }
        public override void WriteJson(JsonWriter writer, HashSet<string> value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            if (value != null)
            {
                foreach (string ft in value)
                {
                    writer.WriteValue(ft);
                }
            }
            writer.WriteEndArray();
        }
    }
}
