using System.Collections.Generic;
using M5x.Consul.Transaction;
using Newtonsoft.Json;

namespace M5x.Consul.KV
{
    /// <summary>
    ///     KVTxnResponse  is used to return the results of a transaction.
    /// </summary>
    public class KvTxnResponse
    {
        public KvTxnResponse()
        {
            Results = new List<KVPair>();
            Errors = new List<TxnError>();
        }

        internal KvTxnResponse(TxnResponse txnRes)
        {
            if (txnRes == null)
            {
                Results = new List<KVPair>(0);
                Errors = new List<TxnError>(0);
                return;
            }

            if (txnRes.Results == null)
            {
                Results = new List<KVPair>(0);
            }
            else
            {
                Results = new List<KVPair>(txnRes.Results.Count);
                foreach (var txnResult in txnRes.Results) Results.Add(txnResult.Kv);
            }

            if (txnRes.Errors == null)
                Errors = new List<TxnError>(0);
            else
                Errors = txnRes.Errors;
        }

        [JsonIgnore] public bool Success { get; internal set; }

        [JsonProperty] public List<TxnError> Errors { get; internal set; }

        [JsonProperty] public List<KVPair> Results { get; internal set; }
    }
}