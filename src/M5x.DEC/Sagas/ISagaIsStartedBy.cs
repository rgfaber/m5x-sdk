using M5x.DEC.Schema;

namespace M5x.DEC.Sagas;

public interface
    ISagaIsStartedByAsync<TAggregate, in TIdentity, in TAggregateEvent> : ISagaHandlesAsync<TAggregate, TIdentity,
        TAggregateEvent>
    where TAggregateEvent : class, IAggregateEvent<TAggregate, TIdentity>
    where TAggregate : IAggregateRoot<TIdentity>
    where TIdentity : IIdentity
{
}

public interface
    ISagaIsStartedBy<TAggregate, in TIdentity, in TAggregateEvent> : ISagaHandles<TAggregate, TIdentity,
        TAggregateEvent>
    where TAggregateEvent : class, IAggregateEvent<TAggregate, TIdentity>
    where TAggregate : IAggregateRoot<TIdentity>
    where TIdentity : IIdentity
{
}