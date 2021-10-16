using M5x.Schemas;

namespace M5x.DEC
{
    public interface IEmit<in TAggregateEvent>
        where TAggregateEvent : IAggregateEvent
    {
        void Emit(TAggregateEvent aggregateEvent);
    }
}