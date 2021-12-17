using Newtonsoft.Json;

namespace M5x.Consul.Agent;

/// <summary>
///     AgentCheckRegistration is used to register a new check
/// </summary>
public class AgentCheckRegistration : AgentServiceCheck
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Id { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Name { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string Notes { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string ServiceId { get; set; }
}