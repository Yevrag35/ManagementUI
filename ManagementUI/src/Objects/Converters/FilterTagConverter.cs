using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagementUI.Converters
{
    public class FilterTagConverter : JsonConverter<SortedSet<FilterTag>>
    {
        public override SortedSet<FilterTag> ReadJson(JsonReader reader, Type objectType, SortedSet<FilterTag> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            SortedSet<FilterTag> tl = new SortedSet<FilterTag>();
            JToken token = JToken.ReadFrom(reader);
            if (token.Type == JTokenType.Array && token is JArray jar)
            {
                foreach (JToken item in jar)
                {
                    if (item.Type == JTokenType.String)
                    {
                        tl.Add(new FilterTag(item.ToObject<string>()));
                    }
                }
            }
            return tl;
        }
        public override void WriteJson(JsonWriter writer, SortedSet<FilterTag> value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            if (value != null)
            {
                foreach (FilterTag ft in value)
                {
                    writer.WriteValue(ft.Tag);
                }
            }
            writer.WriteEndArray();
        }
    }
}
