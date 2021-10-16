using System;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace M5x.RabbitMQ
{
    public static class Inject
    {
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services)
        {
            return services
                .AddSingleton<IConnectionFactory, ConnectionFactory>(x => new ConnectionFactory
                {
                    Uri = new Uri(RabbitMqConfig.Url),
                    UserName = RabbitMqConfig.UserName,
                    Password = RabbitMqConfig.Password
                })
                .AddTransient<IRabbitMqClient, RabbitMqClient>();
        }
    }
}