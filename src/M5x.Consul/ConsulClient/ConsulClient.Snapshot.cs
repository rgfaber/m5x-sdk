using System;
using M5x.Consul.Interfaces;

namespace M5x.Consul.ConsulClient
{
    public partial class ConsulClient : IConsulClient
    {
        private Lazy<Snapshot.Snapshot> _snapshot;

        /// <summary>
        ///     Catalog returns a handle to the snapshot endpoints
        /// </summary>
        public ISnapshotEndpoint Snapshot => _snapshot.Value;
    }
}