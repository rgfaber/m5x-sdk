using System.Collections.Generic;
using M5x.Schemas;

namespace M5x.DEC.Persistence
{
    internal interface IEventSourcingAggregate<TAggregateId>
    {
        long Version { get; }
        void ApplyEvent(IEvent<TAggregateId> @event, long version);
        IEnumerable<IEvent<TAggregateId>> GetUncommittedEvents();

        void ClearUncommittedEvents();
//        IEnumerable<IAggregateEvent<TAggregateId>> Load();
    }
}