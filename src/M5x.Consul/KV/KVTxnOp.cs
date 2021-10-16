namespace M5x.Consul.KV
{
    /// <summary>
    ///     KVTxnOp defines a single operation inside a transaction.
    /// </summary>
    public class KVTxnOp
    {
        public KVTxnOp(string key, KVTxnVerb verb)
        {
            Key = key;
            Verb = verb;
        }

        public KVTxnVerb Verb { get; set; }
        public string Key { get; set; }
        public byte[] Value { get; set; }
        public ulong Flags { get; set; }
        public ulong Index { get; set; }
        public string Session { get; set; }
    }
}