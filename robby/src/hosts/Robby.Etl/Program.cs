using System.Threading.Tasks;
using M5x.Config;
using Microsoft.Extensions.Hosting;
using Robby.Etl.Infra;
using Robby.Etl.Infra.Game;

namespace Robby.Etl
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            DotEnv.FromEmbedded();
            await new HostBuilder()
                .ConfigureServices((hostContext, services) => { services.AddRobbyEtlInfra(); })
                .RunConsoleAsync();
        }
    }
}