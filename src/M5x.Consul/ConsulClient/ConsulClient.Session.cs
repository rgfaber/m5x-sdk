using System;
using M5x.Consul.Interfaces;

namespace M5x.Consul.ConsulClient
{
    public partial class ConsulClient : IConsulClient
    {
        private Lazy<Session.Session> _session;

        /// <summary>
        ///     Session returns a handle to the session endpoint
        /// </summary>
        public ISessionEndpoint Session => _session.Value;
    }
}