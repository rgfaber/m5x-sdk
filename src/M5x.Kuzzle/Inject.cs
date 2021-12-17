using M5x.Kuzzle.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.Kuzzle;

public static class Inject
{
    public static IServiceCollection AddKuzzle(this IServiceCollection services)
    {
        return services
            .AddSingleton<IKuzzleBuilder, KuzzleBuilder>();
    }
}