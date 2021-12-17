using System;
using System.Collections.Generic;
using M5x.Consul.Agent;
using M5x.Consul.Health;
using Microsoft.AspNetCore.Builder;

namespace M5x.Consul.Chassis;

public interface IDiscoInspector
{
    HealthCheck[] HealthState { get; }
    IEnumerable<Uri> Lookups { get; }
    IEnumerable<AgentService> AgentServices { get; }
    Dictionary<string, string[]> CatalogServices { get; }
    void RunDiscovery(IApplicationBuilder app, string key);
    void RunConsulHealth(IApplicationBuilder app);
}