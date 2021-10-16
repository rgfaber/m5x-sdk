using M5x.Consul.KV;

namespace M5x.Consul.Transaction
{
    internal class TxnResult
    {
        public KVPair Kv { get; set; }
    }
}