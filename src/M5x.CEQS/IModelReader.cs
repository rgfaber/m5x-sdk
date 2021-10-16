using System.Collections.Generic;
using System.Threading.Tasks;
using M5x.CEQS.Schema;

namespace M5x.CEQS
{
    public interface IModelReader<TQuery, TReadModel> 
        where TQuery : Query
        where TReadModel : IReadEntity
    {
        
        Task<IEnumerable<TReadModel>> FindAllAsync(TQuery qry);

        Task<TReadModel> GetByIdAsync(string id);
    }
}