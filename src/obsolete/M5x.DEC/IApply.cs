using M5x.Schemas;

namespace M5x.DEC
{
    public interface IApply<in TAggregateEvent>
        where TAggregateEvent : IAggregateEvent
    {
        void Apply(TAggregateEvent aggregateEvent);
    }
}