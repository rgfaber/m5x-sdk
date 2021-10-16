using M5x.DEC.Infra.Web;
using M5x.DEC.Schema;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Robby.Client.Infra.Queries
{
    public static class ById
    {
        
        public static IServiceCollection AddByIdClient(this IServiceCollection services)
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

        
        
        public interface IClient : Http.IQueryClient<Contract.Game.Queries.ById.Qry, Contract.Game.Queries.ById.Rsp>
        {
        }

        [Endpoint(Contract.Game.Config.Endpoints.ById)]
        internal class Client : Http.QueryClient<Contract.Game.Queries.ById.Qry, Contract.Game.Queries.ById.Rsp>, IClient
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