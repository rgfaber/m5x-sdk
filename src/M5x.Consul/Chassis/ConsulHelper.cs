using System;
using System.Collections.Generic;
using System.Linq;
using M5x.Consul.Agent;
using M5x.Consul.Health;
using M5x.Consul.Interfaces;

namespace M5x.Consul.Chassis
{
    public static class ConsulHelper
    {
        /// <summary>
        ///     Discovers the service.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>List&lt;Uri&gt;.</returns>
        public static List<Uri> DiscoverService(string key)
        {
            using (var consulClient = CreateClient())
            {
                var services = consulClient.Agent.Services().Result.Response;
                var uris = (from service in services
                        where service.Value.ID.Contains(key)
                        select new Uri($"{service.Value.Address}:{service.Value.Port}"))
                    .ToList();
                return uris;
            }
        }

        public static List<AgentService> DiscoverAgentServices(string key)
        {
            using (var consulClient = CreateClient())
            {
                var res = new List<AgentService>();
                var services = consulClient.Agent.Services().Result.Response;
                foreach (var agentService in services)
                    if (agentService.Value.ID.Contains(key))
                        res.Add(agentService.Value);
                return res;
            }
        }


        private static IConsulClient CreateClient()
        {
            return new ConsulClient.ConsulClient(c => c.Address = new Uri(ConsulConfig.Address));
        }


        public static HealthCheck[] CheckHealth()
        {
            using (var clt = CreateClient())
            {
                return clt.Health.State(HealthStatus.Any).Result.Response;
            }
        }


        public static Dictionary<string, string[]> RunCatalogServices(string key)
        {
            using (var clt = CreateClient())
            {
                return clt.Catalog.Services().Result.Response;
            }
        }
    }
}