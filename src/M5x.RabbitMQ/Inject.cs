using System;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using MassTransit.ExtensionsDependencyInjectionIntegration;

namespace M5x.RabbitMQ
{
    public static class Inject
    {
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, Action<IServiceCollectionBusConfigurator> configureMassTransit)
        {
            return services
                .AddMassTransit(configureMassTransit)
                .AddSingleton<IConnectionFactory, ConnectionFactory>(
                    x => new ConnectionFactory
                    {
                        Uri = new Uri(RabbitMqConfig.Url),
                        UserName = RabbitMqConfig.UserName,
                        Password = RabbitMqConfig.Password
                    })
                .AddTransient<IRabbitMqClient, RabbitMqClient>();
        }
    }
}