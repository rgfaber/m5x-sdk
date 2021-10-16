using M5x.DEC.Infra;
using M5x.Serilog;
using M5x.Stan;
using Microsoft.Extensions.DependencyInjection;

namespace Robby.Etl.Infra.Game
{
    public static partial class Inject
    {
        public static IServiceCollection AddRobbyEtlInfra(this IServiceCollection services)
        {
            return services
                .AddConsoleLogger()
                .AddCouchClient()
                .AddJetStreamInfra()
                .AddTransient<IGameDb, GameDb>()
                .AddInitializeEtl()
                .AddUpdateDescriptionEtl()
                .AddUpdateDimensionsEtl()
                .AddUpdatePopulationEtl();
        }
    }
}