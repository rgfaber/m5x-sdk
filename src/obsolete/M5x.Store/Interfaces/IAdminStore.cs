using System.Threading.Tasks;
using M5x.DEC.Persistence;
using M5x.Schemas;

namespace M5x.Store.Interfaces
{
    public interface IAdminStore<TReadModel> : IStore where TReadModel : IReadEntity
    {
        Task<XResponse> Initialize(string filterDoc = "");
        Task<XResponse> Replicate(string filterDoc = "");
        Task<XResponse> Compact();
        Task<XResponse> Delete();

        Task<bool> DbExists();
    }
}