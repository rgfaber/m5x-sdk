using System;
using EventStore.Client;
using Grpc.Core;
using M5x.EventStore.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.EventStore;

public static class Inject
{
    public static IServiceCollection AddDECEsClients(this IServiceCollection services)
    {
        return services?
            .AddEventStore(s =>
            {
                s.ConnectivitySettings.Insecure = EventStoreConfig.Insecure;
                s.ConnectivitySettings.Address = new Uri(EventStoreConfig.Uri);
                s.DefaultCredentials = new UserCredentials(EventStoreConfig.UserName, EventStoreConfig.Password);
                if (EventStoreConfig.UseTls) s.ChannelCredentials = new SslCredentials();
            })
            .AddSingleton<IEsPersistentSubscriptionsClient, EsPersistentSubscriptionsClient>()
            .AddSingleton<IEsClient, EsClient>();
    }


    private static IServiceCollection AddEventStore(this IServiceCollection services,
        Action<EventStoreClientSettings>? clientSettings)
    {
        return services
            .AddEventStoreClient(clientSettings)
            .AddEventStoreOperationsClient(clientSettings)
            .AddEventStorePersistentSubscriptionsClient(clientSettings)
            .AddEventStoreProjectionManagementClient(clientSettings)
            .AddEventStoreUserManagementClient(clientSettings);
    }
}