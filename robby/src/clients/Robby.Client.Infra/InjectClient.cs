using M5x.DEC.Infra.Web;
using M5x.Serilog;
using M5x.Stan;
using Microsoft.Extensions.DependencyInjection;
using Robby.Client.Infra.Features;
using Robby.Client.Infra.Queries;

namespace Robby.Client.Infra
{
    public static class InjectClient
    {
        public static IServiceCollection AddRobbyRequesters(this IServiceCollection services)
        {
            return services?
                .AddConsoleLogger()
                .AddJetStreamInfra()
                .AddTransient<Initialize.IRequester, Initialize.Requester>();
        }


        public static IServiceCollection AddRobbyQueryClients(this IServiceCollection services)
        {
            return services?
                .AddConsoleLogger()
                .AddByIdClient()
                .AddFirst20Client();
        }


        public static IServiceCollection AddRobbyHopeClients(this IServiceCollection services)
        {
            return services?
                .AddConsoleLogger()
                .AddSingleton<Http.IHopeOptions, Http.HttpOptions>(x => new Http.HttpOptions
                {
                    BaseUrl = Config.HopeOptions.Url,
                    Timeout = 1000
                })
                .AddSingleton<IHopeFactory, HopeFactory>()
                .AddInitializeClient();
        }
    }
}