using System;
using Newtonsoft.Json;

namespace M5x.Consul.Session;

public class SessionBehaviorConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, ((SessionBehavior)value).Behavior);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        JsonSerializer serializer)
    {
        var behavior = (string)serializer.Deserialize(reader, typeof(string));
        switch (behavior)
        {
            case "release":
                return SessionBehavior.Release;
            case "delete":
                return SessionBehavior.Delete;
            default:
                throw new ArgumentException("Unknown session behavior value during deserialization");
        }
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(SessionBehavior);
    }
}