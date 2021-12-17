using System;
using M5x.Consul.Interfaces;

namespace M5x.Consul.ConsulClient;

public partial class ConsulClient : IConsulClient
{
    private Lazy<Catalog.Catalog> _catalog;

    /// <summary>
    ///     Catalog returns a handle to the catalog endpoints
    /// </summary>
    public ICatalogEndpoint Catalog => _catalog.Value;
}