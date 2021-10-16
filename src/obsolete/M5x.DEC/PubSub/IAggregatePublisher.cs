using System.Threading.Tasks;

namespace M5x.DEC.PubSub
{
    public interface IAggregatePublisher
    {
        Task PublishAsync<T>(T publishedEvent);
    }
}