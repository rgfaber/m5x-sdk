using System;
using Newtonsoft.Json;

namespace M5x.Consul.Utilities
{
    public class DurationTimespanConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, ((TimeSpan)value).ToGoDuration());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            return Extensions.FromGoDuration((string)serializer.Deserialize(reader, typeof(string)));
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(TimeSpan)) return true;
            return false;
        }
    }
}