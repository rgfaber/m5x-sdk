using M5x.DEC.Infra;
using M5x.Serilog;
using M5x.Stan;
using Microsoft.Extensions.DependencyInjection;
using Robby.Cmd.Infra;
using Robby.Cmd.Infra.Game;
using Robby.Etl.Infra;
using Robby.Etl.Infra.Game;

namespace Robby.Infra.IntegrationTests
{
    public static class InjectEtl
    {
        public static IServiceCollection AddRobbyEtlTestInfra(this IServiceCollection services)
        {
            return services
                .AddConsoleLogger()
                .AddCouchClient()
                .AddStanInfraFromK8S()
                .AddInitializeEtl();
        }


        public static IServiceCollection AddClientTestInfra(this IServiceCollection services)
        {
            return services;
        }

        public static IServiceCollection AddRobbyCmdTestInfra(this IServiceCollection services)
        {
            return services
                .AddConsoleLogger()
                .AddCouchClient()
                .AddStan()
                .AddInitializeContextCmd();
        }
    }
}