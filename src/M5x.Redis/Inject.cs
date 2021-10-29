using System;
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
                    return new ConfigurationOptions()
                    {
                        Ssl = Config.UseSsl,
                        Password = Config.Password,
                        User = Config.User,
                        AllowAdmin = Config.AllowAdmin
                    };
                })
                .AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>()
                ;
        }
    }
}
