using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace M5x.DEC.PubSub
{
    public interface IDECBus : IDisposable
    {
        void Subscribe<T>(Action<T> handler);

        void Subscribe<T>(Func<T, Task> handler);

        Task PublishAsync<T>(T publishedEvent);
    }


    internal class DECBus : IDECBus
    {
        private static readonly AsyncLocal<Dictionary<Type, List<object>>> handlers =
            new();

        public Dictionary<Type, List<object>> Handlers =>
            handlers.Value ?? (handlers.Value = new Dictionary<Type, List<object>>());

        public async Task PublishAsync<T>(T publishedEvent)
        {
            foreach (var handler in GetHandlersOf<T>())
                try
                {
                    switch (handler)
                    {
                        case Action<T> action:
                            action(publishedEvent);
                            break;
                        case Func<T, Task> action:
                            await action(publishedEvent);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
        }

        public void Subscribe<T>(Action<T> handler)
        {
            GetHandlersOf<T>().Add(handler);
        }

        public void Subscribe<T>(Func<T, Task> handler)
        {
            GetHandlersOf<T>().Add(handler);
        }

        public void Dispose()
        {
            foreach (var handlersOfT in Handlers.Values) handlersOfT.Clear();
            Handlers.Clear();
        }

        private ICollection<object> GetHandlersOf<T>()
        {
            return Handlers.GetValueOrDefault(typeof(T)) ?? (Handlers[typeof(T)] = new List<object>());
        }
    }
}