using System;
using M5x.Consul.Interfaces;

namespace M5x.Consul.ConsulClient
{
    public partial class ConsulClient : IConsulClient
    {
        private Lazy<Health.Health> _health;

        /// <summary>
        ///     Health returns a handle to the health endpoint
        /// </summary>
        public IHealthEndpoint Health => _health.Value;
    }
}