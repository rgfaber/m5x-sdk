using System.Collections.Generic;
using M5x.DEC.Infra.RabbitMq;
using M5x.DEC.Persistence;
using M5x.DEC.PubSub;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Serilog;

namespace M5x.DEC.TestKit.Tests.SuT
{
    public static class Inject
    {
        public static IServiceCollection AddMySubscriber(this IServiceCollection services)
        {
            return services?
                .AddHostedService<MySubscriber>();
        }
    }


    public class MySubscriber : RMqSubscriber<MyID, MyFact>
    {
        public MySubscriber(IConnectionFactory connFact,
            IDECBus bus,
            IEnumerable<IFactHandler<MyID, MyFact>> handlers,
            ILogger logger) : base(connFact,
            bus,
            handlers,
            logger)
        {
        }
    }
}