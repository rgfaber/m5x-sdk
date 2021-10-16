using M5x.DEC.Persistence;
using M5x.DEC.Persistence.EventStore;
using M5x.DEC.PubSub;
using Robby.Domain;

namespace Robby.Infra
{
    public interface IEventRepo : IEventRepository<Aggregate.Root, Schema.Aggregate.ID>
    {
    }
    
    internal class EventRepo: EventRepository<Aggregate.Root, Schema.Aggregate.ID>, IEventRepo
    {
        public EventRepo(IEventStore eventStore, IAggregatePublisher publisher) 
            : base(eventStore, publisher)
        {
        }
    }
    
    
    
}