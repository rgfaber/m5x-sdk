using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting;
using Serilog.Formatting.Elasticsearch;

namespace M5x.Chassis.Service.Api
{
    public static class ServiceBuilder
    {
        public static IWebHostBuilder Create<T>(string[] args, ITextFormatter logFormatter = null)
            where T : class
        {
            if (logFormatter == null)
                logFormatter = new ElasticsearchJsonFormatter();
            var addr = ApiConfig.GetApiAddress();
            var port = ApiConfig.GetApiPort();
            var conf = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .Build();
            return WebHost
                .CreateDefaultBuilder(args)
                .ConfigureLogging(logging => logging.ClearProviders()
                    .AddSerilog())
                .UseStartup<T>();
        }


        public static IWebHost Build<T>(string[] args, ITextFormatter logFormatter = null)
            where T : class
        {
            return Create<T>(args, logFormatter)
                .Build();
        }
    }
}