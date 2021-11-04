using CouchDB.Driver;
using FakeItEasy;
using M5x.DEC.TestKit.Tests.SuT.Infra.CouchDb;
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
                .AddTransient<IMyCouchReader, MyCouchReader>()
                .AddTransient<IMyCouchSingletonReader, MyCouchSingletonReader>(); ;
        }
        
        
        public static IServiceCollection AddMyFakeReaders(this IServiceCollection services)
        {
            return services
                .AddConsoleLogger()
                .AddMyFakeDb()
                .AddTransient<IMyCouchReader, MyCouchReader>()
                .AddTransient<IMyCouchSingletonReader, MyCouchSingletonReader>(); ;
        }
        
        
        
        
    }
}