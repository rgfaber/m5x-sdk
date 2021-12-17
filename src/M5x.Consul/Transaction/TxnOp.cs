using M5x.Consul.KV;

namespace M5x.Consul.Transaction;

internal class TxnOp
{
    public KVTxnOp Kv { get; set; }
}