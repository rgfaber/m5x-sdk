using M5x.DEC.Infra.STAN;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client;
using Robby.Domain.Game;
using Serilog;
using Aggregate = Robby.Schema.Aggregate;

namespace Robby.Cmd.Infra.Game
{
    public static class UpdateDescription
    {
        public static IServiceCollection AddUpdateDescriptionCmd(this IServiceCollection services)
        {
            return services?
                .AddUpdateDescriptionActor()
                .AddTransient<Domain.Game.UpdateDescription.IEmitter, Emitter>();
        }

        internal class Emitter : STANEmitter<Schema.Game.ID, Contract.Game.Features.UpdateDescription.Fact>,
            Domain.Game.UpdateDescription.IEmitter
        {
            public Emitter(IEncodedConnection conn, ILogger logger) : base(conn, logger)
            {
            }
        }
    }
}