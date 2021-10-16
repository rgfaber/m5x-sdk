using M5x.Schemas;

namespace M5x.DEC.Events
{
    public interface IEventUpcaster<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        IAggregateEvent<TAggregate, TIdentity> Upcast(IAggregateEvent<TAggregate, TIdentity> aggregateEvent);
    }
}