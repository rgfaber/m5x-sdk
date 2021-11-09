using M5x.Config;
using M5x.DEC.Persistence;
using M5x.DEC.PubSub;
using M5x.DEC.TestKit.Integration.Cmd;
using M5x.DEC.TestKit.Tests.SuT;
using M5x.DEC.TestKit.Tests.SuT.Domain;
using M5x.DEC.TestKit.Tests.SuT.Infra.RabbitMq;
using M5x.RabbitMQ;
using M5x.Serilog;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Serilog;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Tests
{
    public class MyEmitterTests : EmitterTests<
        IConnectionFactory,
        IMyEventEmitter,
        MySubscriber,
        MyID,
        MyEvent,
        MyFact>
    {
        public MyEmitterTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        protected override void Initialize()
        {
            Connection = Container.GetRequiredService<IConnectionFactory>();
            Bus = Container.GetRequiredService<IDECBus>();
            Emitter = Container.GetRequiredService<IMyEventEmitter>();
            Logger = Container.GetRequiredService<ILogger>();
            var meta = MyBogus.Schema.Meta.Generate();
            var payload = MyBogus.Schema.Payload;
            var cmd = MyCommand.New(meta, TestConstants.CORRELATION_ID, payload);
            TestEvents.OutEvent = MyEvent.New(cmd);
            FactHandler = Container.GetRequiredService<IFactHandler<MyID, MyFact>>();
            Executor = Container.GetRequiredService<IHostExecutor>();
            Subscriber = Container.GetHostedService<MySubscriber>();
            TestFacts.OutFact = MyFact.New(meta, TestConstants.CORRELATION_ID, payload);
        }

        protected override void SetTestEnvironment()
        {
            DotEnv.FromEmbedded();
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
            services
                .AddHostExecutor()
                .AddTransient<IFactHandler<MyID, MyFact>, TheFactHandler>()
                .AddConsoleLogger()
                .AddDECBus()
                .AddSingletonRMq()
                .AddMyEventEmitter()
                .AddMySubscriber();
        }

    }
}