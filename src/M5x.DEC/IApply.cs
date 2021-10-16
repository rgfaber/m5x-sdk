using M5x.DEC.Events;

namespace M5x.DEC
{
    public interface IApply<in TAggregateEvent>
        where TAggregateEvent : IAggregateEvent
    {
        void Apply(TAggregateEvent evt);
    }
}