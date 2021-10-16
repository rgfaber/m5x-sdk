using System;
using System.Threading.Tasks;
using M5x.DEC.Persistence;
using M5x.Schemas;
using Serilog;

namespace M5x.DEC.Infra.CouchDb
{
    public abstract class CouchWriter<TAggregateId, TEvent, TReadModel> :
        IModelWriter<TAggregateId, TEvent, TReadModel>
        where TAggregateId : IAggregateID
        where TEvent : IEvent<TAggregateId>
        where TReadModel : IReadEntity
    {
        protected readonly ILogger Logger;

        public CouchWriter(ICouchStore<TReadModel> store, ILogger logger)
        {
            Store = store;
            Logger = logger;
        }

        protected ICouchStore<TReadModel> Store { get; set; }

        public async Task HandleAsync(TEvent @event)
        {
            try
            {
                await UpdateAsync(@event);
            }
            catch (Exception e)
            {
                Logger?.Fatal(e.Message);
            }
        }

        public abstract Task<TReadModel> UpdateAsync(TEvent @event);
    }
}