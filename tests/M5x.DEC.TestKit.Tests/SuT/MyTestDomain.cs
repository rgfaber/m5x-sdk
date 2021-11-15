using FakeItEasy;
using M5x.DEC.Schema;
using M5x.DEC.TestKit.Tests.SuT.Domain;
using M5x.Serilog;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.DEC.TestKit.Tests.SuT
{
    public static class MyTestDomain
    {
        public static MyAggregate Aggregate = MyBogus.Domain.Aggregate.Generate();

        public static MyCommand Command = MyCommand.New(AggregateInfo.New(MyTestSchema.TestID.Value),
            MyTestSchema.TEST_CORRELATION_ID,
            MyTestSchema.Payload);

        public static MyEvent Event = MyEvent.New(Command);

        public static IServiceCollection AddMyFakeActor(this IServiceCollection services)
        {
            var aFakeEventStream = A.Fake<IMyEventStream>();
            var aFakeEmitter = A.Fake<IMyEventEmitter>();
            return services?
                .AddConsoleLogger()
                .AddDECBus()
                .AddTransient<IMyActor, MyActor>()
                .AddTransient(x => aFakeEventStream)
                .AddTransient(x => aFakeEmitter);
        }

        public static IServiceCollection AddMyActor(this IServiceCollection services)
        {
            var aFakeEventStream = A.Fake<IMyEventStream>();
            var aFakeEmitter = A.Fake<IMyEventEmitter>();
            return services?
                .AddConsoleLogger()
                .AddDECBus()
                .AddEventStoreClient()
                .AddTransient<IMyActor, MyActor>()
                .AddTransient(x => aFakeEventStream)
                .AddTransient(x => aFakeEmitter);
        }
    }
}