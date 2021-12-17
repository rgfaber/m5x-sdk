using System;
using System.Linq;
using System.Reflection;
using M5x.Consul.KV;
using Newtonsoft.Json;

namespace M5x.Consul.Utilities;

public class KvPairConverter : JsonConverter
{
    private static readonly Lazy<string[]> ObjProps =
        new(() => typeof(KVPair).GetRuntimeProperties().Select(p => p.Name).ToArray());

    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        JsonSerializer serializer)
    {
        var result = new KVPair();
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.StartObject) continue;
            if (reader.TokenType == JsonToken.EndObject) return result;
            if (reader.TokenType == JsonToken.PropertyName)
            {
                var jsonPropName = reader.Value.ToString();
                var propName =
                    ObjProps.Value.FirstOrDefault(p => p.Equals(jsonPropName, StringComparison.OrdinalIgnoreCase));

                var pi = result.GetType().GetRuntimeProperty(propName);

                if (jsonPropName.Equals("Flags", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(reader.ReadAsString()))
                    {
                        var val = Convert.ToUInt64(reader.Value);
                        pi.SetValue(result, val, null);
                    }
                }
                else if (jsonPropName.Equals("Value", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(reader.ReadAsString()))
                    {
                        var val = Convert.FromBase64String(reader.Value.ToString());
                        pi.SetValue(result, val, null);
                    }
                }
                else
                {
                    if (reader.Read())
                    {
                        var convertedValue = Convert.ChangeType(reader.Value, pi.PropertyType);
                        pi.SetValue(result, convertedValue, null);
                    }
                }
            }
        }

        return result;
    }

    public override bool CanConvert(Type objectType)
    {
        if (objectType == typeof(KVPair)) return true;
        return false;
    }
}