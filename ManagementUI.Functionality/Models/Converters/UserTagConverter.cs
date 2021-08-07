using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using ManagementUI.Functionality.Models.Collections;

namespace ManagementUI.Functionality.Models.Converters
{
    public class UserTagConverter : JsonConverter<SortedSet<UserTag>>
    {
        private static TagIdDictionary _tagsToIds = new TagIdDictionary();
        public static int TagCount => _tagsToIds.Count;
        public static SortedSet<UserTag> GetLoadedTags()
        {
            var set = new SortedSet<UserTag>(_tagsToIds);
            _tagsToIds.Clear();

            return set;
        }

        public override SortedSet<UserTag> ReadJson(JsonReader reader, Type objectType, SortedSet<UserTag> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var set = new SortedSet<UserTag>();
            if (reader.TokenType == JsonToken.StartArray)
            {
                while (reader.Read() && reader.TokenType == JsonToken.String)
                {
                    string key = (string)reader.Value;
                    if (!_tagsToIds.ContainsKey(key))
                    {
                        _tagsToIds.Add(key);
                    }

                    set.Add(_tagsToIds[key]);
                }
            }

            return set;
        }

        public override void WriteJson(JsonWriter writer, SortedSet<UserTag> value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            if (null == value || value.Count <= 0)
            {
                writer.WriteEndArray();
                return;
            }

            foreach (UserTag tag in value.OrderBy(x => x.Value))
            {
                writer.WriteValue(tag.Value);
            }

            writer.WriteEndArray();
        }
    }
}