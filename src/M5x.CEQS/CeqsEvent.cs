using EventFlow.Aggregates;
using EventFlow.Core;
using M5x.CEQS.Schema;

namespace M5x.CEQS
{
    public abstract class CeqsEvent<TAggregate, TAggregateId, TFact> :
        AggregateEvent<TAggregate, TAggregateId>, ICeqsEvent<TAggregateId>
        where TAggregate : IAggregateRoot<TAggregateId>
        where TAggregateId : IIdentity
        where TFact : IFact<TAggregateId>

    {
        protected CeqsEvent()
        {
        }

        protected CeqsEvent(TAggregateId aggregateId)
        {
            AggregateId = aggregateId;
        }

        public TAggregateId AggregateId { get; set; }
    }

    public interface ICeqsEvent<TAggregateId> : IAggregateEvent
        where TAggregateId : IIdentity
    {
        TAggregateId AggregateId { get; set; }        
    }
    
    public interface ICeqsEvent<TAggregateId,TPayload> : ICeqsEvent<TAggregateId>
        where TAggregateId: IIdentity
        where TPayload:  class
    {
    
    }
}