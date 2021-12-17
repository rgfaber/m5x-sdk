using System;
using M5x.Consul.Interfaces;

namespace M5x.Consul.ConsulClient;

public partial class ConsulClient : IConsulClient
{
    private Lazy<Agent.Agent> _agent;

    /// <summary>
    ///     Agent returns a handle to the agent endpoints
    /// </summary>
    public IAgentEndpoint Agent => _agent.Value;
}