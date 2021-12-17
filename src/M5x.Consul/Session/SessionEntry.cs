using System;
using System.Collections.Generic;
using M5x.Consul.Utilities;
using Newtonsoft.Json;

namespace M5x.Consul.Session;

public class SessionEntry
{
    public SessionEntry()
    {
        Checks = new List<string>();
    }

    public ulong CreateIndex { get; set; }

    public string ID { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Name { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Node { get; set; }

    public List<string> Checks { get; set; }

    [JsonConverter(typeof(NanoSecTimespanConverter))]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public TimeSpan? LockDelay { get; set; }

    [JsonConverter(typeof(SessionBehaviorConverter))]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public SessionBehavior Behavior { get; set; }

    [JsonConverter(typeof(DurationTimespanConverter))]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public TimeSpan? Ttl { get; set; }

    public bool ShouldSerializeId()
    {
        return false;
    }

    public bool ShouldSerializeCreateIndex()
    {
        return false;
    }

    public bool ShouldSerializeChecks()
    {
        return Checks != null && Checks.Count != 0;
    }
}