using System;
using Newtonsoft.Json;

namespace M5x.Consul.ACL;

public class AclTypeConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, ((ACLType)value).Type);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        JsonSerializer serializer)
    {
        var type = (string)serializer.Deserialize(reader, typeof(string));
        switch (type)
        {
            case "client":
                return ACLType.Client;
            case "management":
                return ACLType.Management;
            default:
                throw new ArgumentOutOfRangeException("serializer", type,
                    "Unknown ACL token type value found during deserialization");
        }
    }

    public override bool CanConvert(Type objectType)
    {
        if (objectType == typeof(ACLType)) return true;
        return false;
    }
}