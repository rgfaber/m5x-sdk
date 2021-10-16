using M5x.DEC.Infra.EventStore;
using M5x.Serilog;
using M5x.Stan;
using Microsoft.Extensions.DependencyInjection;
using Robby.Domain.Game;

namespace Robby.Cmd.Infra.Game
{
    public static class InjectCmd
    {
        public static IServiceCollection AddRobbyCmdInfra(this IServiceCollection services)
        {
            return services?
                .AddConsoleLogger()
                .AddJetStreamInfra()
                .AddSingletonEventStore()
                .AddTransient<IGameStream, GameStream>()
                .AddInitializeContextCmd()
                .AddUpdateDescriptionCmd()
                .AddUpdateDimensionsCmd()
                .AddUpdatePopulationCmd();
        }
    }
}