using M5x.DEC.Infra.Web;
using M5x.DEC.Schema;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Robby.Client.Infra.Queries
{
    public static class First20
    {
        public static IServiceCollection AddFirst20Client(this IServiceCollection services)
        {
            return services
                .AddSingleton<Http.IQueryOptions, Http.HttpOptions>(x => new Http.HttpOptions
                {
                    BaseUrl = Config.QueryOptions.Url,
                    Timeout = 1000
                })
                .AddSingleton<IQueryFactory, QueryFactory>()
                .AddTransient<IClient, Client>();
        }


        public interface IClient : Http.IQueryClient<Contract.Game.Queries.First20.Qry, Contract.Game.Queries.First20.Rsp>
        {
        }

        [Endpoint(Contract.Game.Config.Endpoints.First20)]
        internal class Client : Http.QueryClient<Contract.Game.Queries.First20.Qry, Contract.Game.Queries.First20.Rsp>, IClient
        {
            public Client(IQueryFactory fact,
                Http.IQueryOptions httpOptions,
                ILogger logger,
                Authenticate.IClient authenticator = null) : base(fact,
                httpOptions,
                logger,
                authenticator)
            {
            }
        }
    }
}