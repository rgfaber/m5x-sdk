using System;
using EventFlow.EventStores;
using M5x.EventStore;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.CEQS.Infra.EventStore
{
    public static class Inject
    {
        public static IServiceCollection AddScopedEventStore(this IServiceCollection services,
            Func<string> eventStoreUri = null)
        {
            return services?
                .AddEventStreamClient()
                .AddScoped<IEventPersistence, EventPersistence>();
        }


        public static IServiceCollection AddSingletonEventStore(this IServiceCollection services,
            Func<string> eventStoreUri = null)
        {
            return services?
                .AddEventStreamClient()
                .AddSingleton<IEventPersistence, EventPersistence>();
        }
    }
}