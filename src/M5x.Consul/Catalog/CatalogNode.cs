using System.Collections.Generic;
using M5x.Consul.Agent;

namespace M5x.Consul.Catalog
{
    public class CatalogNode
    {
        public CatalogNode()
        {
            Services = new Dictionary<string, AgentService>();
        }

        public Node Node { get; set; }
        public Dictionary<string, AgentService> Services { get; set; }
    }
}