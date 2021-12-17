using System;
using Newtonsoft.Json;

namespace M5x.Consul.PreparedQuery;

/// <summary>
///     QueryDNSOptions controls settings when query results are served over DNS.
/// </summary>
public class QueryDnsOptions
{
    /// <summary>
    ///     TTL is the time to live for the served DNS results.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public TimeSpan? Ttl { get; set; }
}