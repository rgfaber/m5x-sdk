using System;
using M5x.DEC.Events;
using M5x.DEC.Schema;

namespace M5x.DEC.Sagas
{
    public interface IDomainEvent
    {
        Type AggregateType { get; }
        Type IdentityType { get; }
        Type EventType { get; }
        long AggregateSequenceNumber { get; }
        Metadata Metadata { get; }
        DateTimeOffset Timestamp { get; }

        IIdentity GetIdentity();
        IAggregateEvent GetAggregateEvent();
    }

    public interface IDomainEvent<TAggregate, out TIdentity> : IDomainEvent
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        TIdentity AggregateIdentity { get; }
    }

    public interface IDomainEvent<TAggregate, out TIdentity, out TAggregateEvent> : IDomainEvent<TAggregate, TIdentity>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TAggregateEvent : class, IAggregateEvent<TAggregate, TIdentity>
    {
        TAggregateEvent AggregateEvent { get; }
    }
}