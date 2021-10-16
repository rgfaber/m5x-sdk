using System.Threading.Tasks;
using M5x.DEC.Persistence;
using M5x.Schemas;

namespace M5x.Store.Interfaces
{
    public interface IStoreBuilder<TReadModel> where TReadModel : IReadEntity
    {
        IStore Build(string dbName);
        IAdminStore<TReadModel> BuildAdmin(string dbName);
        IAdminStore<TReadModel> BuildAdmin<TReadModel>() where TReadModel : IReadEntity;
        IStore Build(string remoteAddress, string dbName);
        IAdminStore<TReadModel> BuildAdmin(string remoteAddress, string dbName);
    }


    public interface IPingStore
    {
        Task<string> GetStoreInfo();
    }
}