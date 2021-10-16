using System.Collections.Generic;
using System.Threading.Tasks;
using M5x.CEQS.Schema;
using Serilog;

namespace M5x.CEQS.Infra.CouchDb
{
    public abstract class CouchReader<TQuery, TReadModel> : IModelReader<TQuery, TReadModel>
        where TQuery : Query
        where TReadModel : IReadEntity
    {
        protected readonly ILogger Logger;
        
        protected readonly ICouchStore<TReadModel> Store;

        public CouchReader(ICouchStore<TReadModel> store, ILogger logger)
        {
            Store = store;
            Logger = logger;
        }
        
        public abstract Task<IEnumerable<TReadModel>> FindAllAsync(TQuery qry);
        
        public async Task<TReadModel> GetByIdAsync(string id)
        {
            return await Store.GetByIdAsync(id);
        }
    }
}