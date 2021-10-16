using System;
using System.Threading.Tasks;
using EventFlow.Core;
using M5x.CEQS.Schema;
using Serilog;

namespace M5x.CEQS.Infra.CouchDb
{
    public abstract class CouchWriter<TAggregateId, TFact, TReadModel> 
        : IModelWriter<TAggregateId, TFact, TReadModel>
        where TFact : IFact<TAggregateId>
        where TReadModel : IReadEntity
        where TAggregateId : IIdentity
    {
        protected readonly ILogger Logger;

        public CouchWriter(ICouchStore<TReadModel> store, ILogger logger)
        {
            Store = store;
            Logger = logger;
        }

        protected ICouchStore<TReadModel> Store { get; set; }

        public abstract Task<TReadModel> UpdateAsync(TFact fact);

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
    }
}