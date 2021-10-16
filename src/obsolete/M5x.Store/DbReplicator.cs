using System.Collections.Generic;
using System.Threading.Tasks;
using M5x.DEC.Persistence;
using M5x.Schemas;
using M5x.Store.Interfaces;

namespace M5x.Store
{
    public class DbReplicator<TReadModel> : IReplicateDb where TReadModel : IReadEntity
    {
        private readonly IStoreBuilder<TReadModel> _builder;

        public DbReplicator(IStoreBuilder<TReadModel> builder)
        {
            _builder = builder;
        }

        public async Task<Dictionary<string, XResponse>> Replicate(string name)
        {
            if (!StoreConfig.CanReplicate) return null;
            using var store = _builder.BuildAdmin(name);
            return await store.OpenDb();
        }
    }
}