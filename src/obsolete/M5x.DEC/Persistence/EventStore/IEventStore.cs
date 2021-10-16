using System.Collections.Generic;
using System.Threading.Tasks;
using M5x.Schemas;

namespace M5x.DEC.Persistence.EventStore
{
    public interface IEventStore
    {
        Task<IEnumerable<StoreEvent<TAggregateId>>> ReadEventsAsync<TAggregateId>(TAggregateId id)
            where TAggregateId : IAggregateID;
        Task<AppendResult> AppendEventAsync<TAggregateId>(IEvent<TAggregateId> @event)
            where TAggregateId : IAggregateID;

        Task<IEnumerable<StoreEvent<TAggregateId>>> ReadEventsSlicedAsync<TAggregateId>(TAggregateId id, int sliceSize)
            where TAggregateId : IAggregateID;

    }
}