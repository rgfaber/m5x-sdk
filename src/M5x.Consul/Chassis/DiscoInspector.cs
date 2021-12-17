using System;
using System.Collections.Generic;
using M5x.Consul.Agent;
using M5x.Consul.Health;
using M5x.Networking;
using Microsoft.AspNetCore.Builder;

namespace M5x.Consul.Chassis;

public class DiscoInspector : IDiscoInspector
{
    public string Me => NetUtils.GetLocalIpAddress().ToString();

    public void RunDiscovery(IApplicationBuilder app, string key)
    {
        AgentServices = ConsulHelper.DiscoverAgentServices(key);
    }

    public IEnumerable<Uri> Lookups { get; private set; }
    public IEnumerable<AgentService> AgentServices { get; private set; }

    public void RunConsulHealth(IApplicationBuilder app)
    {
        HealthState = ConsulHelper.CheckHealth();
    }

    public HealthCheck[] HealthState { get; private set; }
    public Dictionary<string, string[]> CatalogServices { get; private set; }

    public void RunCatalogServices(string key)
    {
        CatalogServices = ConsulHelper.RunCatalogServices(key);
    }
}