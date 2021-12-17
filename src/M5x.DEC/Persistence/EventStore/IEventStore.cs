using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using M5x.DEC.Events;
using M5x.DEC.Schema;

namespace M5x.DEC.Persistence.EventStore;

public interface IEventStore
{
    Task<IEnumerable<StoreEvent<TAggregateId>>> ReadEventsAsync<TAggregateId>(TAggregateId id)
        where TAggregateId : IIdentity;

    Task<AppendResult> AppendEventAsync<TAggregateId>(IEvent<TAggregateId> @event)
        where TAggregateId : IIdentity;

    Task<IEnumerable<StoreEvent<TAggregateId>>> ReadEventsSlicedAsync<TAggregateId>(TAggregateId id,
        int sliceSize, long startPos) where TAggregateId : IIdentity;

    IAsyncEnumerable<StoreEvent<TAggregateId>> ReadAllEventsAsync<TAggregateId>(
        CancellationToken cancellationToken = default)
        where TAggregateId : IIdentity;
}