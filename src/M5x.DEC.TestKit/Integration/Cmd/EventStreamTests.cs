using M5x.DEC.Persistence;
using M5x.DEC.Schema;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration.Cmd
{
    public abstract class EventStreamTests<TAggregate, TAggregateId, TEventStream> : IoCTestsBase 
        where TAggregateId : IIdentity 
        where TAggregate : IAggregate<TAggregateId>
        where TEventStream: IEventStream<TAggregate,TAggregateId>

    {
        private IEventStream<TAggregate, TAggregateId> _eventStream;

        public EventStreamTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        protected override void Initialize()
        {
            _eventStream = Container.GetService<TEventStream>();
        }

        [Fact]
        public void Needs_EventStream()
        {
            Assert.NotNull(_eventStream);
        }

    }
}