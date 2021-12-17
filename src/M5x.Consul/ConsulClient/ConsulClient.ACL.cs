using System;
using M5x.Consul.ACL;
using M5x.Consul.Interfaces;

namespace M5x.Consul.ConsulClient;

public partial class ConsulClient : IConsulClient
{
    private Lazy<Acl> _acl;

    /// <summary>
    ///     ACL returns a handle to the ACL endpoints
    /// </summary>
    public IAclEndpoint ACL => _acl.Value;
}