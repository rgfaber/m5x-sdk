using M5x.Config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Robby.Cmd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DotEnv.FromEmbedded();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}