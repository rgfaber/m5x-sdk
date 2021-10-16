using M5x.DEC.PubSub;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.DEC
{
    public static class Inject
    {
        public static IServiceCollection AddDEC(this IServiceCollection services)
        {
            return services?
                .AddTransient<IAggregatePublisher, AggregatePubSub>()
                .AddTransient<IAggregateSubscriber, AggregatePubSub>();
        }
    }
}