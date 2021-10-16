using M5x.Consul.Agent;
using M5x.Consul.Catalog;

namespace M5x.Consul.Health
{
    /// <summary>
    ///     ServiceEntry is used for the health service endpoint
    /// </summary>
    public class ServiceEntry
    {
        public Node Node { get; set; }
        public AgentService Service { get; set; }
        public HealthCheck[] Checks { get; set; }
    }
}