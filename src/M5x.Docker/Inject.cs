using M5x.Docker.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.Docker;

public static class Inject
{
    public static IServiceCollection AddDockerEnvironment(this IServiceCollection services)
    {
        return services?
            .AddSingleton<IDockerEnvironment, DockerEnvironment>();
    }
}