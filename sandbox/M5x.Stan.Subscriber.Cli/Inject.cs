using AutoMapper;
using M5x.DEC.Infra;
using M5x.Publisher.Contract;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.Stan.Subscriber.Cli
{
    public static class Inject
    {
        public static IServiceCollection AddRafTestedListener(this IServiceCollection services)
        {
            return services?
                .AddSingletonDECInfra()
                .AddTransient<IRafTestedHandler, RafTestedConsoleWriter>()
                .AddHostedService<RafTestedListener>();
        }


        public static IServiceCollection AddRequestMappers(this IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TestRafReq, TestRaf>();
            });
            return services?
                .AddSingleton(mapperConfig.CreateMapper());
        }


        public static IServiceCollection AddTrivialResponder(this IServiceCollection services)
        {
            return services
                .AddSingletonDECInfra()
                .AddTransient<ITrivialResponder, TrivialResponder>();
        }
        



        public static IServiceCollection AddRafTestedEmitter(this IServiceCollection services)
        {
            return services?
                .AddTransient<IRafTestedEmitter, RafTestedEmitter>();
        }


        public static IServiceCollection AddTestRafResponder(this IServiceCollection services)
        {
            return services?
                .AddSingletonDECInfra()
                .AddTransient<IRafEventRepo, RafEventRepo>()
                .AddRequestMappers()
                .AddRafTestedEmitter()
                .AddTransient<ITestRafActor, TestRafActor>()
                .AddHostedService<TestRafResponder>();
        }
        
    }
}