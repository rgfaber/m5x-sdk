using System.Threading.Tasks;
using M5x.DEC.Persistence;
using M5x.Schemas;
using M5x.Store.Interfaces;

namespace M5x.Store
{
    internal class DbCompacter<TReadModel> : ICompactDb where TReadModel : IReadEntity
    {
        private readonly IStoreBuilder<TReadModel> _store;

        public DbCompacter(IStoreBuilder<TReadModel> store)
        {
            _store = store;
        }

        public async Task<XResponse> Compact(string name)
        {
            using (var db = _store.BuildAdmin<TReadModel>())
            {
                return await db.Compact();
            }
        }
    }
}