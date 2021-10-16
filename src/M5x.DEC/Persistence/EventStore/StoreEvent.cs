using M5x.DEC.Events;
using M5x.DEC.Schema;

namespace M5x.DEC.Persistence.EventStore
{
    public class StoreEvent<TAggregateId> where TAggregateId : IIdentity
    {
        public StoreEvent(IEvent<TAggregateId> @event, long eventNumber)
        {
            Event = @event;
            EventNumber = eventNumber;
        }

        public long EventNumber { get; }

        public IEvent<TAggregateId> Event { get; }
    }
    
    

    // public class StoreEvent<TAggregateId, TPayload>
    //     where TAggregateId : IIdentity
    //     where TPayload : IPayload
    // {
    //     public StoreEvent(IEvent<TAggregateId, TPayload> @event, long eventNumber)
    //     {
    //         Event = @event;
    //         EventNumber = eventNumber;
    //     }
    //
    //     public long EventNumber { get; }
    //
    //     public IEvent<TAggregateId, TPayload> Event { get; }
    // }
}