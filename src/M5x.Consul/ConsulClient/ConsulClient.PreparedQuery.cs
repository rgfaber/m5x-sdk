using System;
using M5x.Consul.Interfaces;

namespace M5x.Consul.ConsulClient;

public partial class ConsulClient : IConsulClient
{
    private Lazy<PreparedQuery.PreparedQuery> _preparedquery;

    /// <summary>
    ///     Catalog returns a handle to the catalog endpoints
    /// </summary>
    public IPreparedQueryEndpoint PreparedQuery => _preparedquery.Value;
}