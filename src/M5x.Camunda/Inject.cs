using M5x.Camunda.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.Camunda;

public static class Inject
{
    public static IServiceCollection AddBpm(this IServiceCollection services)
    {
        return services?
            .AddSingleton<IBpmClient, BpmClient>()
            .AddSingleton<IEngineClient, EngineClient>();
    }

    public static IApplicationBuilder UseBpm(this IApplicationBuilder app)
    {
        return app;
    }
}