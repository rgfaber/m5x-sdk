using System.Reflection;
using System.Threading.Tasks;
using FakeItEasy;
using M5x.DEC.Persistence;
using M5x.DEC.Persistence.EventStore;
using M5x.DEC.PubSub;
using M5x.DEC.Schema;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.Tests
{
    [IDPrefix("my")]
    public record MyID : Identity<MyID>
    {
        public MyID(string value) : base(value)
        {
        }
    }

    internal class MyRoot : AggregateRoot<MyID>
    {
        public MyRoot(MyID id) : base(id)
        {
        }

        public MyRoot()
        {
        }
    }

    internal interface IMyEventStream : IEventStream<MyRoot, MyID>
    {
    }

    internal class MyEventStream : EventStream<MyRoot, MyID>, IMyEventStream
    {
        public MyEventStream(IEventStore eventStore, IDECBus publisher) : base(eventStore, publisher)
        {
        }
    }

    public class PersistenceTests : IoCTestsBase
    {
        private static readonly MyID _someId = MyID.New;


        public PersistenceTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        [Fact]
        public void Should_EventStreamGetByIdShouldReturnAggregateWithThatId()
        {
            var stream = Container.GetService<IMyEventStream>();
            var agg = stream.GetById(_someId);
            Assert.Equal(_someId.Value, agg.Id.Value);
        }

        [Fact]
        public void Should_CreateEmptyAggregateWithIDCallReturnsAggregateWithId()
        {
            var someId = MyID.New;
            var stream = Container.GetService<IMyEventStream>();
            var method = typeof(EventStream<MyRoot, MyID>)
                .GetMethod("CreateEmptyAggregate", BindingFlags.Instance | BindingFlags.NonPublic);
            var c = (MyRoot)method?.Invoke(stream, new object[] { someId });
            Assert.Equal(someId.Value, c?.Id.Value);
        }

        protected override void Initialize()
        {
            
        }

        protected override void SetTestEnvironment()
        {
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
            var someEventStore = A.Fake<IEventStore>();
            services
                .AddSingleton(someEventStore)
                .AddDECBus()
                .AddTransient<IMyEventStream, MyEventStream>();
        }
    }
}