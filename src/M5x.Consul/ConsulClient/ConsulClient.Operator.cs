using System;
using M5x.Consul.Interfaces;

namespace M5x.Consul.ConsulClient
{
    public partial class ConsulClient : IConsulClient
    {
        private Lazy<Operator.Operator> _operator;

        /// <summary>
        ///     Operator returns a handle to the operator endpoints.
        /// </summary>
        public IOperatorEndpoint Operator => _operator.Value;
    }
}