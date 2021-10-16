using CouchDB.Driver;
using FakeItEasy;
using M5x.Serilog;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.DEC.TestKit.Tests.SuT
{
    public static class MyTestQryInfra
    {
        public static IServiceCollection AddMyReaders(this IServiceCollection services)
        {
            return services
                .AddConsoleLogger()
                .AddMyDb()
                .AddTransient<IMyReader, MyReader>()
                .AddTransient<IMySingletonReader, MySingletonReader>(); ;
        }
        
        
        public static IServiceCollection AddMyFakeReaders(this IServiceCollection services)
        {
            return services
                .AddConsoleLogger()
                .AddMyFakeDb()
                .AddTransient<IMyReader, MyReader>()
                .AddTransient<IMySingletonReader, MySingletonReader>(); ;
        }
        
        
        
        
    }
}