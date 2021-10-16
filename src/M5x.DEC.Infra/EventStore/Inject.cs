using System;
using M5x.DEC.Persistence.EventStore;
using M5x.EventStore;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.DEC.Infra.EventStore
{
    public static class Inject
    {
        public static IServiceCollection AddScopedEventStore(this IServiceCollection services,
            Func<string> eventStoreUri = null)
        {
            return services?
                .AddDECEsClients()
                .AddScoped<IEventStore, EventStoreDb>();
        }


        public static IServiceCollection AddSingletonEventStore(this IServiceCollection services,
            Func<string> eventStoreUri = null)
        {
            return services?
                .AddDECEsClients()
                .AddSingleton<IEventStore, EventStoreDb>();
        }
    }
}