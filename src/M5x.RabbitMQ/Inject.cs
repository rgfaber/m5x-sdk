using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace M5x.RabbitMQ
{
    public static class Inject
    {
        public static IServiceCollection AddSingletonRMq(this IServiceCollection services)
        {
            return services
                .AddSingleton<IConnectionFactory, ConnectionFactory>(p => new ConnectionFactory
                {
                    DispatchConsumersAsync = Config.DispatchConsumerAsync,
                    VirtualHost = Config.VHost,
                    HostName = Config.Host,
                    UserName = Config.User,
                    Password = Config.Password
                });
        }
    }
}