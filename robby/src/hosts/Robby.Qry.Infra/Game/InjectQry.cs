using M5x.DEC.Infra;
using M5x.Serilog;
using Microsoft.Extensions.DependencyInjection;

namespace Robby.Qry.Infra.Game
{
    public static class InjectQry
    {
        public static IServiceCollection AddRobbyQryInfra(this IServiceCollection services)
        {
            return services?
                .AddConsoleLogger()
                .AddCouchClient()
                .AddTransient<IGameDb, GameDb>()
                .AddFirst20Qry()
                .AddById();
        }
    }
}