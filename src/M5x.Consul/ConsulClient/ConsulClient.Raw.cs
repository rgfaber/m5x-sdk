using System;
using M5x.Consul.Interfaces;

namespace M5x.Consul.ConsulClient;

public partial class ConsulClient : IConsulClient
{
    private Lazy<Raw.Raw> _raw;

    /// <summary>
    ///     Raw returns a handle to query endpoints
    /// </summary>
    public IRawEndpoint Raw => _raw.Value;
}