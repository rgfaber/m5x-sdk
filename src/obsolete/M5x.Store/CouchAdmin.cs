using System;
using System.Threading.Tasks;
using M5x.Schemas;
using MyCouch;
using MyCouch.Requests;
using Serilog;


namespace M5x.Store
{
    public abstract class CouchAdmin<TReadModel> : ICouchAdmin<TReadModel> where TReadModel : IReadEntity
    {
        private readonly ILogger _logger;

        public CouchAdmin(ILogger logger)
        {
            _logger = logger;
        }

        private string DbName => GetDbName();


        public async Task<XResponse> ReplicateAsync(string filterDoc = "")
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
                    Reason = $"Replication {repId} Exists"
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


        public async Task<XResponse> CompactAsync()
        {
            using var client = new MyCouchServerClient(StoreConfig.LocalServer);
            var request = new CompactDatabaseRequest(DbName);
            var res = await client.Databases.CompactAsync(request);
            return res.ToXResponse();
        }

        public async Task<XResponse> DeleteAsync()
        {
            using var client = new MyCouchServerClient(StoreConfig.LocalServer);
            var res = await client.Databases.DeleteAsync(DbName);
            return res.ToXResponse();
        }


        public async Task<XResponse> InitializeAsync(string filterDoc = "")
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

        public string GetDbName()
        {
            var atts = (DbNameAttribute[]) typeof(TReadModel).GetCustomAttributes(typeof(DbNameAttribute), true);
            if (atts.Length == 0) throw new Exception($"{typeof(TReadModel)} does not have a DBName Attribute!");
            return atts[0].DbName;
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

    public interface ICouchAdmin<TReadModel> where TReadModel : IReadEntity
    {
        Task<XResponse> CompactAsync();
        Task<XResponse> ReplicateAsync(string filterDoc = "");
        Task<XResponse> InitializeAsync(string filterDoc = "");
        Task<XResponse> DeleteAsync();
    }
}