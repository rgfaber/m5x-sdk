using System;
using Newtonsoft.Json;

namespace M5x.Consul.KV
{
    public class KvTxnVerbTypeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, ((KVTxnVerb)value).Operation);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var status = (string)serializer.Deserialize(reader, typeof(string));
            switch (status)
            {
                case "set":
                    return KVTxnVerb.Set;
                case "delete":
                    return KVTxnVerb.Delete;
                case "delete-cas":
                    return KVTxnVerb.DeleteCas;
                case "delete-tree":
                    return KVTxnVerb.DeleteTree;
                case "cas":
                    return KVTxnVerb.Cas;
                case "lock":
                    return KVTxnVerb.Lock;
                case "unlock":
                    return KVTxnVerb.Unlock;
                case "get":
                    return KVTxnVerb.Get;
                case "get-tree":
                    return KVTxnVerb.GetTree;
                case "check-session":
                    return KVTxnVerb.CheckSession;
                case "check-index":
                    return KVTxnVerb.CheckIndex;
                default:
                    throw new ArgumentException("Invalid KVTxnOpType value during deserialization");
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(KVTxnVerb);
        }
    }
}