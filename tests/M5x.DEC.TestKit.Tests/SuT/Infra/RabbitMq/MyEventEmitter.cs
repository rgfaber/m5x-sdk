using M5x.DEC.Infra.RabbitMq;
using M5x.DEC.TestKit.Tests.SuT.Domain;
using Microsoft.Extensions.DependencyInjection;
using Polly.Retry;
using RabbitMQ.Client;
using Serilog;
using YamlDotNet.Serialization;

namespace M5x.DEC.TestKit.Tests.SuT.Infra.RabbitMq
{
    public static partial class Inject
    {
        public static IServiceCollection AddMyEventEmitter(this IServiceCollection services)
        {
            return services?
                .AddTransient<IMyEventEmitter, MyEventEmitter>();
        }
    }


    internal class MyEventEmitter : RMqEmitter<MyID,MyEvent,MyFact>, IMyEventEmitter
    {
        public MyEventEmitter(
            IConnectionFactory connFact,
            ILogger logger,
            AsyncRetryPolicy retryPolicy = null) : base(connFact,
            logger,
            retryPolicy)
        {
        }

        protected override MyFact ToFact(MyEvent @event)
        {
            return MyFact.New(@event.Meta, @event.CorrelationId, @event.Payload);
        }
    }
}