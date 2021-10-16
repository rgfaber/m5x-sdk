using M5x.DEC.Infra;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.Stan.Publisher.Cli
{
    public static class Inject
    {
        public static IServiceCollection AddTestRafRequester(this IServiceCollection services)
        {
            return services?
                .AddSingletonDECInfra()
                .AddTransient<ITestRafRequester, TestRafRequester>();
        }



        public static IServiceCollection AddTrivialRequester(this IServiceCollection services)
        {
            return services?
                .AddSingletonDECInfra()
                .AddTransient<ITrivialRequester, TrivialRequester>();
        }
        
        
    }
}