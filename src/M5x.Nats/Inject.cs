using System;
using M5x.Nats.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MyNatsClient;

namespace M5x.Nats;

public static class Inject
{
    [Obsolete("Please use M5x.Stan sdk instead")]
    public static IServiceCollection AddNats(this IServiceCollection services)
    {
        return services?
            .AddSingleton(x =>
            {
                var conn = new ConnectionInfo(BusConfig.Host, BusConfig.Port);
                if (!string.IsNullOrWhiteSpace(BusConfig.User))
                    conn.Credentials = new Credentials(BusConfig.User, BusConfig.Password);
                return conn;
            })
            .AddSingleton<INatsClient, NatsClient>()
            .AddSingleton<IBusBuilder, BusBuilder>();
    }
}