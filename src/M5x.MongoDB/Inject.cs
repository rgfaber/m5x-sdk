using Microsoft.Extensions.DependencyInjection;

namespace M5x.MongoDB;

public static class Inject
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services)
    {
        return services;
    }
}