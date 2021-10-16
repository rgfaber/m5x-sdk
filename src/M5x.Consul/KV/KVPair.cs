using M5x.Consul.Utilities;
using Newtonsoft.Json;

namespace M5x.Consul.KV
{
    /// <summary>
    ///     KVPair is used to represent a single K/V entry
    /// </summary>
#if !NET45
    [JsonConverter(typeof(KvPairConverter))]
#endif
    public class KVPair
    {
        public KVPair(string key)
        {
            Key = key;
        }

        internal KVPair()
        {
        }

        public string Key { get; set; }

        public ulong CreateIndex { get; set; }
        public ulong ModifyIndex { get; set; }
        public ulong LockIndex { get; set; }
        public ulong Flags { get; set; }

        public byte[] Value { get; set; }
        public string Session { get; set; }

        internal void Validate()
        {
            ValidatePath(Key);
        }

        internal static void ValidatePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new InvalidKeyPairException("Invalid key. Key path is empty.");
            if (path[0] == '/')
                throw new InvalidKeyPairException(
                    $"Invalid key. Key must not begin with a '/': {path}");
        }
    }
}