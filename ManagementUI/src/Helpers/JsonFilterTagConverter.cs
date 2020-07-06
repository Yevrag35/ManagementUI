using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagementUI.Converters
{
    public class JsonFilterTagConverter : JsonConverter<FilterTag>
    {
        public override FilterTag ReadJson(JsonReader reader, Type objectType, FilterTag existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            FilterTag ret = default;
            JToken jtok = JToken.ReadFrom(reader);
            if (jtok.Type == JTokenType.String)
            {
                ret = new FilterTag(jtok.ToObject<string>(), false);
            }
            return ret;
        }

        public override void WriteJson(JsonWriter writer, FilterTag value, JsonSerializer serializer)
        {
            if (!string.IsNullOrWhiteSpace(value.Tag))
            {
                writer.WriteValue(value.Tag);
            }
        }
    }
}
