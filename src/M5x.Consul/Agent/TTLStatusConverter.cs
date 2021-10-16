using System;
using Newtonsoft.Json;

namespace M5x.Consul.Agent
{
    public class TtlStatusConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, ((TtlStatus)value).Status);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var status = (string)serializer.Deserialize(reader, typeof(string));
            switch (status)
            {
                case "pass":
                    return TtlStatus.Pass;
                case "passing":
                    return TtlStatus.Pass;
                case "warn":
                    return TtlStatus.Warn;
                case "warning":
                    return TtlStatus.Warn;
                case "fail":
                    return TtlStatus.Critical;
                case "critical":
                    return TtlStatus.Critical;
                default:
                    throw new ArgumentException("Invalid TTL status value during deserialization");
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TtlStatus);
        }
    }
}