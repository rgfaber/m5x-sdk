using System.Threading;
using System.Threading.Tasks;
using M5x.DEC.Events;
using M5x.DEC.Schema;

namespace M5x.DEC
{
    
    public interface IEventHandler {}
    
    
    public interface IEventHandler<TAggregateId, in TEvent> : IEventHandler
        where TAggregateId : IIdentity
        where TEvent : IEvent<TAggregateId>
    {
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
    }

    // public interface IEventHandler<TAggregate, TAggregateId, TEvent>
    //     where TAggregateId: IIdentity 
    //     where TEvent : IEvent<TAggregateId>
    //     where TAggregate : IAggregate<TAggregateId>
    // {
    //     Task HandleAsync(IEventStream<TAggregate, TAggregateId> eventStream, TEvent @event);
    // }
}