using M5x.Schemas;

namespace M5x.DEC.Persistence.EventStore
{
    public class StoreEvent<TAggregateId>
    {
        public StoreEvent(IEvent<TAggregateId> @event, long eventNumber)
        {
            Event = @event;
            EventNumber = eventNumber;
        }

        public long EventNumber { get; }

        public IEvent<TAggregateId> Event { get; }
    }

    
}