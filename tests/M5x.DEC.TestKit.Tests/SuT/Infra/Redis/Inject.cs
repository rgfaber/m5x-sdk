using Microsoft.Extensions.DependencyInjection;

namespace M5x.DEC.TestKit.Tests.SuT.Infra.Redis
{
    public static class Inject
    {
        public static IServiceCollection AddMyRedisWriter(this IServiceCollection services)
        {
            return services?
                .AddTransient<IMyRedisEventWriter, MyRedisEtlWriter>();
        }

        public static IServiceCollection AddMyRedisReaders(this IServiceCollection services)
        {
            return services?
                .AddTransient<IMyRedisReader, MyRedisReader>()
                .AddTransient<IMySingletonRedisReader, MySingletonRedisReader>();
        }
    }
}