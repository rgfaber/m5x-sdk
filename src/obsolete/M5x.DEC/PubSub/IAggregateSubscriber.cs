using System;
using System.Threading.Tasks;

namespace M5x.DEC.PubSub
{
    public interface IAggregateSubscriber
    {
        void Subscribe<T>(Action<T> handler);

        void Subscribe<T>(Func<T, Task> handler);
    }
}