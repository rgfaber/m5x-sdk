using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using M5x.Consul.Agent;
using M5x.Consul.Interfaces;

namespace M5x.Consul
{
    internal class UriPool : IUriPool
    {
        private readonly IConsulClient _consul;


        private IDictionary<string, AgentService> _pool;

        public UriPool(IConsulClient consul, CancellationToken control = default)
        {
            _consul = consul;
            RefreshToken = control;
        }

        public CancellationToken RefreshToken { get; }


        public async Task<Uri> GetUri(string key)
        {
            if (_pool == null)
            {
                _pool = await DiscoverService();
                RefreshPool();
            }

            var subPool = _pool.Where(x => x.Key.Contains(key))
                .ToDictionary(p => p.Key, p => p.Value);
            var r = new Random().Next(subPool.Count);
            var s = subPool.ElementAt(r).Value;
            return new Uri($"{s.Address}:{s.Port}");
        }

        private void RefreshPool()
        {
            Parallel.Invoke(async () =>
            {
                while (!RefreshToken.IsCancellationRequested) _pool = await DiscoverService();
            });
        }

        /// <summary>
        ///     Discovers the service.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>List&lt;Uri&gt;.</returns>
        private async Task<IDictionary<string, AgentService>> DiscoverService()
        {
            return (await _consul.Agent.Services()).Response;
        }
    }
}