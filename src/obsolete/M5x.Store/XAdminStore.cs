using System.Threading.Tasks;
using M5x.DEC.Persistence;
using M5x.Schemas;
using M5x.Store.Interfaces;
using MyCouch;
using MyCouch.Requests;

namespace M5x.Store
{
    internal class XAdminStore<TReadModel> : XStore<TReadModel>, IAdminStore<TReadModel> where TReadModel : IReadEntity
    {
        public XAdminStore(string serverAddress, string dbName = null)
            : base(serverAddress, dbName)
        {
        }


        public async Task<XResponse> Replicate(string filterDoc = "")
        {
            if (!StoreConfig.CanReplicate) return null;
            if (DbName.ToLower() == "_replicator") return null;
            if (StoreConfig.LocalSource == StoreConfig.ClusterSource) return null;
            using var client = new MyCouchServerClient(StoreConfig.LocalServer);
            var repId = $"up:{DbName}";
            if (await ReplicationExists(repId))
                return new XResponse
                {
                    Id = repId,
                    DbName = DbName,
                    IsSuccess = false,
                    Reason = "Replication DbExists"
                };
            var source = $"{StoreConfig.StrictLocalhostServer}/{DbName}";
            var target = $"{StoreConfig.ClusterServer}/{DbName}";
            var request = new ReplicateDatabaseRequest(repId, source, target)
            {
                Continuous = true,
                CreateTarget = true,
                Filter = filterDoc
            };
            var res = await client.Replicator.ReplicateAsync(request);
            return res.ToXResponse();
        }


        public async Task<XResponse> Compact()
        {
            using (var client = new MyCouchServerClient(StoreConfig.LocalServer))
            {
                var request = new CompactDatabaseRequest(DbName);
                var res = await client.Databases.CompactAsync(request);
                return res.ToXResponse();
            }
        }

        public async Task<XResponse> Delete()
        {
            using var client = new MyCouchServerClient(StoreConfig.LocalServer);
            var res = await client.Databases.DeleteAsync(DbName);
            return res.ToXResponse();
        }


        public async Task<XResponse> Initialize(string filterDoc = "")
        {
            if (DbName.ToLower() == "_replicator") return null;
            if (StoreConfig.LocalSource == StoreConfig.ClusterSource) return null;
            using var client = new MyCouchServerClient(StoreConfig.LocalServer);
            var repId = $"down:{DbName}";
            if (await ReplicationExists(repId))
            {
                var resp = new XResponse
                {
                    Id = repId,
                    DbName = DbName,
                    IsSuccess = false,
                    Reason = "Replication DbExists"
                };
                return resp;
            }

            //await ClearReplication(repId);
            var source = $"{StoreConfig.ClusterServer}/{DbName}";
            //var target = $"{StoreConfig.LocalServer}/{DbName}";
            var target = $"{StoreConfig.StrictLocalhostServer}/{DbName}";
            var request = new ReplicateDatabaseRequest(repId, source, target)
            {
                Continuous = true,
                CreateTarget = true,
                Filter = filterDoc
            };
            var res = await client.Replicator.ReplicateAsync(request);
            return res.ToXResponse();
        }


        private async Task<bool> ClearReplication(string id)
        {
            return await new StoreBuilder<TReadModel>()
                .BuildAdmin("_replicator")
                .DeleteAsync(id);
        }


        private async Task<bool> ReplicationExists(string id)
        {
            var res = await new StoreBuilder<TReadModel>()
                .BuildAdmin("_replicator")
                .DocExists(id);
            return res;
        }
    }
}