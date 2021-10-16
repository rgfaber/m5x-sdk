using M5x.DEC.Persistence;
using M5x.DEC.Persistence.EventStore;
using M5x.DEC.PubSub;
using M5x.Publisher.Contract;

namespace M5x.Stan.Subscriber.Cli
{
    
    public interface IRafEventRepo: IEventRepository<RafRoot, RafId>
    {
    }

    
    
    internal class RafEventRepo: EventRepository<RafRoot,RafId>, IRafEventRepo
    {
        public RafEventRepo(IEventStore eventStore, IAggregatePublisher publisher) : base(eventStore, publisher)
        {
        }
    }
}