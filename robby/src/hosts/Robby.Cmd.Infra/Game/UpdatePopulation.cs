using M5x.DEC.Infra.STAN;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client;
using Robby.Domain.Game;
using Robby.Schema;
using Serilog;

namespace Robby.Cmd.Infra.Game
{
    public static class UpdatePopulation
    {
        public static IServiceCollection AddUpdatePopulationCmd(this IServiceCollection services)
        {
            return services
                .AddUpdatePopulationActor()
                .AddTransient<Domain.Game.UpdatePopulation.IEmitter, Emitter>();
        }

        internal class Emitter : STANEmitter<Schema.Game.ID, Contract.Game.Features.UpdatePopulation.Fact>,
            Domain.Game.UpdatePopulation.IEmitter
        {
            public Emitter(IEncodedConnection conn, ILogger logger) : base(conn, logger)
            {
            }
        }
    }
}