using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CouchDB.Driver;
using M5x.DEC.Schema;

namespace M5x.DEC.Infra.CouchDb
{
    public interface ICouchStore<TReadModel> where TReadModel : IReadEntity
    {

        Task<TReadModel> AddOrUpdateAsync(TReadModel entity,
            bool batch = false,
            bool withConflicts = false,
            CancellationToken cancellationToken = default);

        Task<TReadModel> GetByIdAsync(string id);

        Task<TReadModel> GetByMeta(AggregateInfo meta);

        Task<IEnumerable<TReadModel>> RetrieveRecent(int pageNumber, int pageSize);

        Task<TReadModel> DeleteAsync(string id);
    }
}