using Newtonsoft.Json;

namespace M5x.Consul.Transaction;

public class TxnError
{
    [JsonProperty] public int OpIndex { get; private set; }

    [JsonProperty] public string What { get; private set; }
}