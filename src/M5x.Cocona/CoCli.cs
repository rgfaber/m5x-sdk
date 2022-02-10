using Cocona;
using Cocona.Builder;
using Cocona.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.Cocona;

public static class CoCli
{
    public static CoconaAppHostBuilder CreateAppHostBuilder(this IServiceCollection services)
    {
        return CoconaApp.CreateHostBuilder();
    }
    public static CoconaAppBuilder CreateAppBuilder(      
        string[]? args = null,
        Action<CoconaAppOptions>? configureOptions = null)
    {
        return CoconaApp.CreateBuilder(args, configureOptions);
    }

}
