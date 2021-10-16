using M5x.DEC;
using M5x.DEC.Infra.STAN;
using M5x.DEC.Infra.Web;
using M5x.DEC.Schema;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client;
using Robby.Schema;
using Serilog;

namespace Robby.Client.Infra.Features
{
    public static class Initialize
    {
        public static IServiceCollection AddInitializeRequester(this IServiceCollection services)
        {
            return services?
                .AddTransient<IRequester, Requester>();
        }

        public static IServiceCollection AddInitializeClient(this IServiceCollection services)
        {
            return services?
                .AddTransient<IClient, Client>();
        }


        internal class Requester : STANRequester<Game.ID, Contract.Game.Features.Initialize.Hope,
                Contract.Game.Features.Initialize.Feedback>,
            IRequester
        {
            public Requester(IEncodedConnection conn, ILogger logger) : base(conn, logger)
            {
            }
        }

        public interface IRequester : IRequester<Contract.Game.Features.Initialize.Hope,
            Contract.Game.Features.Initialize.Feedback>
        {
        }


        [Endpoint(Contract.Game.Config.Endpoints.Initialize)]
        internal class Client : Http.HopeClient<Contract.Game.Features.Initialize.Hope,
            Contract.Game.Features.Initialize.Feedback>, IClient
        {
            public Client(IHopeFactory fact,
                Http.IHopeOptions hopeOptions,
                ILogger logger,
                Authenticate.IClient authenticator = null) : base(fact,
                hopeOptions,
                logger,
                authenticator)
            {
            }
        }

        public interface IClient : Http.IHopeClient<Contract.Game.Features.Initialize.Hope,
            Contract.Game.Features.Initialize.Feedback>
        {
        }
    }
}