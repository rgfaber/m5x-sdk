using System.Collections.Generic;
using Newtonsoft.Json;

namespace M5x.Consul.Transaction;

internal class TxnResponse
{
    [JsonProperty] internal List<TxnResult> Results { get; set; }

    [JsonProperty] internal List<TxnError> Errors { get; set; }
}