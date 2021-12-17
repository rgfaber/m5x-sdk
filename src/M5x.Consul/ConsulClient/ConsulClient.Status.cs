using System;
using M5x.Consul.Interfaces;

namespace M5x.Consul.ConsulClient;

public partial class ConsulClient : IConsulClient
{
    private Lazy<Status.Status> _status;

    /// <summary>
    ///     Status returns a handle to the status endpoint
    /// </summary>
    public IStatusEndpoint Status => _status.Value;
}