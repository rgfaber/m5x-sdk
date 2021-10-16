using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CouchDB.Driver;
using M5x.CEQS.Schema;

namespace M5x.CEQS.Infra.CouchDb
{
    public interface ICouchStore<TReadModel> where TReadModel : IReadEntity
    {
        ICouchDatabase<CDoc<TReadModel>> Db { get; }

        Task<TReadModel> AddOrUpdateAsync(TReadModel entity,
            bool batch = false,
            bool withConflicts = false,
            CancellationToken cancellationToken = default(CancellationToken));
        
        Task<TReadModel> GetByIdAsync(string id);
        
        Task<IEnumerable<TReadModel>> RetrieveRecent(int pageNumber, int pageSize);
    }
}