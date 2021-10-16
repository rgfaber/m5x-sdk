using System;
using M5x.Consul.Interfaces;

namespace M5x.Consul.ConsulClient
{
    public partial class ConsulClient : IConsulClient
    {
        private Lazy<Event.Event> _event;

        /// <summary>
        ///     Event returns a handle to the event endpoints
        /// </summary>
        public IEventEndpoint Event => _event.Value;
    }
}