using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using M5x.Publisher.Contract;
using M5x.Schemas;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.Stan.Publisher.Cli
{


    class Program
    {
        private static IServiceCollection services = new ServiceCollection();
        private static RafId rafId = RafId.NewId();
        
        static async Task Main(string[] args)
        {
            services.AddTestRafRequester();
//            services.AddTrivialRequester();
            var sp = services.BuildServiceProvider();
            var requester = sp.GetService<ITestRafRequester>();
            long j = 0;
            do
            {
                j++;
                var req = TestRafReq.CreateNew(rafId.Id, GuidFactories.NewCleanGuid);
                var rsp = await requester.RequestAsync(req);
                Console.WriteLine(JsonSerializer.Serialize(rsp));
                Thread.Sleep(2);
            } while (j <= 9999999999);
        }
        
        
    }
}
