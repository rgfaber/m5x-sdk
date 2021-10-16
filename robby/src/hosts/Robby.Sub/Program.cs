using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Robby.Sub
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            await new HostBuilder()
                .ConfigureServices((hostContext, services) => { throw new NotImplementedException(); })
                .RunConsoleAsync();
        }
    }
}