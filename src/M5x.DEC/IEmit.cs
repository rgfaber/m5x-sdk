using M5x.DEC.Events;

namespace M5x.DEC
{
    public interface IEmit<in TAggregateEvent>
        where TAggregateEvent : IAggregateEvent
    {
        void Emit(TAggregateEvent aggregateEvent);
    }
}