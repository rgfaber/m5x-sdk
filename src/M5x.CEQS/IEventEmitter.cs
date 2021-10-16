using EventFlow.Aggregates;
using EventFlow.Core;
using EventFlow.Subscribers;
using M5x.CEQS.Schema;

namespace M5x.CEQS
{
    public interface IEventEmitter<TAggregate, in TAggregateId, in TEvent> 
        : ISubscribeSynchronousTo<TAggregate, TAggregateId, TEvent>
        where TAggregate : IAggregateRoot<TAggregateId> 
        where TAggregateId : IIdentity 
        where TEvent : IAggregateEvent<TAggregate, TAggregateId>
    {
    }
    
    
    
    
}