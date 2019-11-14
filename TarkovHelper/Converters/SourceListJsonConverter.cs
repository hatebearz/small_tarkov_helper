using System;
using System.Collections.Generic;
using Avalonia.Controls.Templates;
using DynamicData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TarkovHelper.Converters
{
    public class SourceListJsonConverter<T>: JsonConverter<SourceList<T>>
    {
        public override void WriteJson(JsonWriter writer, SourceList<T> value, JsonSerializer serializer)
        {
            var token = JToken.FromObject(value.Items);
            token.WriteTo(writer);
        }

        public override SourceList<T> ReadJson(JsonReader reader, Type objectType, SourceList<T> existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var token = JToken.ReadFrom(reader);
            var sourceList = new SourceList<T>();
            foreach (var requirement in token.ToObject<List<T>>())
                sourceList.Add(requirement);
            return sourceList;
        }
    }
}