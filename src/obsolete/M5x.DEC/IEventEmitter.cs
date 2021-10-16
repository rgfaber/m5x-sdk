using M5x.Schemas;

namespace M5x.DEC
{
    public interface IEventEmitter<TAggregateId, TEvent> : IAggregateEventHandler<TAggregateId, TEvent>
        where TEvent : IEvent<TAggregateId>
        where TAggregateId : IAggregateID
    {
    }
}