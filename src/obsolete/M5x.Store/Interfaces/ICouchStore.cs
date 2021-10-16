using System.Threading.Tasks;
using CouchDB.Driver;
using M5x.DEC.Persistence;
using M5x.Schemas;

namespace M5x.Store.Interfaces
{
    public interface ICouchStore<TReadModel> where TReadModel : IReadEntity
    {
        ICouchDatabase<CDoc<TReadModel>> Db { get; }
        Task<TReadModel> AddOrUpdateAsync(TReadModel entity);
        Task<TReadModel> GetByIdAsync(string id);
    }
}