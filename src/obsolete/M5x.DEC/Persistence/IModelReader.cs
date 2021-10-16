using System.Collections.Generic;
using System.Threading.Tasks;
using M5x.Schemas;

namespace M5x.DEC.Persistence
{
    public interface IModelReader<TQuery, TReadModel> 
        where TQuery : Query
        where TReadModel : IReadEntity
    {
        
        Task<IEnumerable<TReadModel>> FindAllAsync(TQuery qry);

        Task<TReadModel> GetByIdAsync(string id);
    }
}