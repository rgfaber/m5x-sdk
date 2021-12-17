using M5x.Consul.Health;
using Newtonsoft.Json;

namespace M5x.Consul.Agent;

/// <summary>
///     AgentCheck represents a check known to the agent
/// </summary>
public class AgentCheck
{
    public string Node { get; set; }
    public string CheckId { get; set; }
    public string Name { get; set; }

    [JsonConverter(typeof(HealthStatusConverter))]
    public HealthStatus Status { get; set; }

    public string Notes { get; set; }
    public string Output { get; set; }
    public string ServiceId { get; set; }
    public string ServiceName { get; set; }
}