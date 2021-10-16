using M5x.DEC.Infra.STAN;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client;
using Robby.Domain.Game;
using Robby.Schema;
using Serilog;

namespace Robby.Cmd.Infra.Game
{
    public static class UpdateDimensions
    {
        public static IServiceCollection AddUpdateDimensionsCmd(this IServiceCollection services)
        {
            return services?
                .AddUpdateDimensionsActor()
                .AddTransient<Domain.Game.UpdateDimensions.IEmitter, Emitter>();
        }

        internal class Emitter : STANEmitter<Schema.Game.ID, Contract.Game.Features.UpdateDimensions.Fact>,
            Domain.Game.UpdateDimensions.IEmitter
        {
            public Emitter(IEncodedConnection conn, ILogger logger) : base(conn, logger)
            {
            }
        }
    }
}