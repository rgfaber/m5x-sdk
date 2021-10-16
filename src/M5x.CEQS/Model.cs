using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates;
using EventFlow.Core;
using EventFlow.ReadStores;
using M5x.CEQS.Schema;

namespace M5x.CEQS
{
    public abstract class Model<TAggregate, TID, TReadEntity, TEvent>
        : Model<TAggregate,TID, TReadEntity>, IAmReadModelFor<TAggregate, TID, TEvent>
        where TAggregate: AggregateRoot<TAggregate,TID>
        where  TID: Identity<TID>
        where TEvent: IAggregateEvent<TAggregate,TID>
        where TReadEntity : IReadEntity
    {
        public abstract void Apply(IReadModelContext context, IDomainEvent<TAggregate, TID, TEvent> domainEvent);
        protected Model(TID aggregateId) : base(aggregateId)
        {
        }
    }


    public abstract class Model<TAggregate, TID, TReadEntity> : IReadModel
        where TAggregate: AggregateRoot<TAggregate, TID>
        where TReadEntity: IReadEntity
        where  TID: Identity<TID>
    {

        public Model()
        {
            
        }
        
        
        protected Model(TID aggregateId)
        {
            AggregateId = aggregateId;
        }

        protected Model(TID aggregateId, TReadEntity state)
        {
            AggregateId = aggregateId;
            State = state;
        }

        public TID AggregateId { get; set; }
        public TReadEntity State { get; set; }
    }
        
    
    
    public abstract class AsyncModel<TAggregate, TID, TReadEntity, TEvent>:
        Model<TAggregate, TID, TReadEntity, TEvent>, IAmAsyncReadModelFor<TAggregate, TID, TEvent>
        where TAggregate: AggregateRoot<TAggregate,TID>
        where  TID: Identity<TID>
        where TEvent: IAggregateEvent<TAggregate,TID>
        where TReadEntity : IReadEntity

    {
        public abstract Task ApplyAsync(IReadModelContext context, IDomainEvent<TAggregate, TID, TEvent> domainEvent,
            CancellationToken cancellationToken);

        protected AsyncModel(TID aggregateId) : base(aggregateId)
        {
        }
    }
    
    
    
    
}