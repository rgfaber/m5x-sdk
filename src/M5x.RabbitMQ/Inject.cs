using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace M5x.RabbitMQ
{
    public static class Inject
    {
        public static IServiceCollection AddTransientRMq(this IServiceCollection services)
        {
            return services
                .AddSingleton<IConnectionFactory, ConnectionFactory>(p => new ConnectionFactory
                {
                    DispatchConsumersAsync = Config.DispatchConsumerAsync,
                    HostName = Config.Host,
                    UserName = Config.User,
                    Password = Config.Password,
                    VirtualHost = Config.VHost
                })
                .AddTransientRMqConnection();
        }

        public static IServiceCollection AddSingletonRMqConnection(this IServiceCollection services)
        {
            return services?
                .AddSingleton(provider =>
                {
                    var fact = provider.GetRequiredService<IConnectionFactory>();
                    return fact.CreateConnection();
                });
        }
        
        public static IServiceCollection AddTransientRMqConnection(this IServiceCollection services)
        {
            return services?
                .AddTransient(provider =>
                {
                    var fact = provider.GetRequiredService<IConnectionFactory>();
                    return fact.CreateConnection();
                });
        }
        
        

        
        
        
        public static IServiceCollection AddSingletonRMq(this IServiceCollection services)
        {
            return services
                .AddSingleton<IConnectionFactory, ConnectionFactory>(p => new ConnectionFactory
                {
                    DispatchConsumersAsync = Config.DispatchConsumerAsync,
                    VirtualHost = Config.VHost,
                    HostName = Config.Host,
                    UserName = Config.User,
                    Password = Config.Password,
                })
                .AddSingletonRMqConnection();
        }
        
        
        
        
        
    }
}