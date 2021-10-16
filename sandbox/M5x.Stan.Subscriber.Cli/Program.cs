using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace M5x.Stan.Subscriber.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services
//                        .AddTrivialResponder();
                    .AddRafTestedEmitter()
                    .AddTestRafResponder()
                    .AddRafTestedListener();
                })
                .RunConsoleAsync();
        }
    }
}
