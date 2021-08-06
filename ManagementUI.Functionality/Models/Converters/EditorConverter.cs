using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagementUI.Functionality.Models.Converters
{
    public abstract class EditorConverter<T> : JsonConverter<T>
        where T : EditorManagerBase, new()
    {
        public string FilePath { get; set; }

        public EditorConverter(string filePath)
        {
            this.FilePath = filePath;
        }

        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var manager = new T();
            manager.AddDefaults(this.FilePath);
            if (reader.TokenType == JsonToken.StartArray)
            {
                foreach (JObject job in JToken.ReadFrom(reader))
                {
                    manager.Add(job);
                }
            }

            return manager;
        }
        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            if (null == value || value.Count <= 0)
            {
                writer.WriteEndArray();
                return;
            }

            foreach (var kvp in value)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("key");
                writer.WriteValue(kvp.Key);
                writer.WritePropertyName("exe");
                writer.WriteValue(kvp.Value.ExePath);
                writer.WritePropertyName("args");
                writer.WriteValue(kvp.Value.Arguments);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }
    }
}
