﻿using System.Collections.Generic;
using M5x.DEC.Events;
using M5x.DEC.Schema;

namespace M5x.DEC.Persistence;
// public interface IEventSourcingAggregate<TAggregateId, TPayload> : IEventSourcingAggregate<TAggregateId>
//     where TAggregateId : IIdentity
//     where TPayload : IPayload
// {
//     void ApplyEvent(IEvent<TAggregateId, TPayload> @event, long version);
//     IEnumerable<IEvent<TAggregateId, TPayload>> GetUncommittedEvents();
// }

public interface IEventSourcingAggregate<TAggregateId> : IAggregateRoot<TAggregateId>
    where TAggregateId : IIdentity
{
    long Version { get; }
    void ApplyEvent(IEvent<TAggregateId> @event, long version);
    void ClearUncommittedEvents();
    void Load(IEnumerable<IEvent<TAggregateId>> aggregateEvents);
    IEnumerable<IEvent<TAggregateId>> GetUncommittedEvents();
}