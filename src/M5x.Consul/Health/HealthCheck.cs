using Newtonsoft.Json;

namespace M5x.Consul.Health;

/// <summary>
///     HealthCheck is used to represent a single check
/// </summary>
public class HealthCheck
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