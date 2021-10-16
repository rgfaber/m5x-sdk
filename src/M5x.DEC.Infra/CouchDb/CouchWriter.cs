using System;
using System.Threading.Tasks;
using M5x.DEC.Persistence;
using M5x.DEC.Schema;
using Serilog;

namespace M5x.DEC.Infra.CouchDb
{
    public abstract class CouchWriter<TAggregateId, TFact, TReadModel> :
        IModelWriter<TAggregateId, TFact, TReadModel>
        where TAggregateId : IIdentity
        where TFact : IFact
        where TReadModel : IReadEntity
    {
        protected readonly ILogger Logger;

        protected CouchWriter(ICouchStore<TReadModel> store, ILogger logger)
        {
            Store = store;
            Logger = logger;
        }

        protected ICouchStore<TReadModel> Store { get; set; }

        public async Task HandleAsync(TFact fact)
        {
            try
            {
                await UpdateAsync(fact);
            }
            catch (Exception e)
            {
                Logger?.Fatal(e.Message);
            }
        }

        public abstract Task<TReadModel> UpdateAsync(TFact fact);
        public async Task<TReadModel> DeleteAsync(string id)
        {
            return await Store.DeleteAsync(id).ConfigureAwait(false);
        }
    }
}