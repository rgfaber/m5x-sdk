using System;
using M5x.DEC.Events;
using M5x.DEC.Schema;

namespace M5x.DEC
{
    public interface IPayloadEvent<out TAggregatetId, out TPayload> : IEvent<TAggregatetId>
        where TAggregatetId : IIdentity
        where TPayload : struct
    {
        TPayload Payload { get; }
    }


    public interface IAggregateEvent<TAggregate, out TAggregateId> : IAggregateEvent
        where TAggregate : IAggregateRoot<TAggregateId>
        where TAggregateId : IIdentity
    {
        Guid EventId { get; }
        TAggregateId AggregateId { get; }
        long AggregateVersion { get; }
    }
}