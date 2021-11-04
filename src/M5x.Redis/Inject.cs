using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace M5x.Redis
{
    public static class Inject
    {
        public static IServiceCollection AddRedis(this IServiceCollection services)
        {
            return services?
                .AddSingleton(p =>
                {
                    var opts = ConfigurationOptions.Parse(Config.ConfigString);
                    opts.Ssl = Config.UseSsl;
                    opts.Password = Config.Password;
                    opts.User = Config.User;
                    opts.AllowAdmin = Config.AllowAdmin;
                    return opts;
                })
                .AddSingleton<IRedisConnectionFactory, RedisConnectionFactory>();
        }

        public static IServiceCollection AddSingletonRedisConnection(this IServiceCollection services)
        {
            return services?
                .AddRedis()
                .AddSingleton(p =>
                {
                    var fact = p.GetRequiredService<IRedisConnectionFactory>();
                    return fact.Connect();
                });
        }

        public static IServiceCollection AddTransientRedisConnection(this IServiceCollection services)
        {
            return services?
                .AddRedis()
                .AddTransient(p =>
                {
                    var fact = p.GetRequiredService<IRedisConnectionFactory>();
                    return fact.Connect();
                });
        }

        public static IServiceCollection AddSingletonRedisDb(this IServiceCollection services)
        {
            return services?
                .AddSingletonRedisConnection()
                .AddSingleton<IRedisDb, RedisDb>();
        }

        public static IServiceCollection AddTransientRedisDb(this IServiceCollection services)
        {
            return services?
                .AddTransientRedisConnection()
                .AddTransient<IRedisDb, RedisDb>();

        }
        
        
        
        
        
        
    }
}