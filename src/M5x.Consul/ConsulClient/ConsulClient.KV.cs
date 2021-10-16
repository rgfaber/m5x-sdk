using System;
using M5x.Consul.Interfaces;
using M5x.Consul.KV;

namespace M5x.Consul.ConsulClient
{
    /// <summary>
    ///     KV is used to return a handle to the K/V apis
    /// </summary>
    public partial class ConsulClient : IConsulClient
    {
        private Lazy<Kv> _kv;

        /// <summary>
        ///     KV returns a handle to the KV endpoint
        /// </summary>
        public IKvEndpoint KV => _kv.Value;
    }
}