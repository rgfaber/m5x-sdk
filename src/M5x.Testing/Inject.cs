using M5x.Testing.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.Testing;

public static class Inject
{
    public static IServiceCollection AddTestHelpers(this IServiceCollection services)
    {
        return services
            .AddHostExecutor()
            .AddSingleton<ITestHelper, TestHelper>();
    }

    public static IServiceCollection AddHostExecutor(this IServiceCollection services)
    {
        return services?
            .AddTransient<IHostExecutor, HostExecutor>();
    }
}