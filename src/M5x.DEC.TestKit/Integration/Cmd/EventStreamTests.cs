using M5x.DEC.Persistence;
using M5x.DEC.Schema;
using M5x.DEC.TestKit.Unit.Domain;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration.Cmd
{
    public abstract class EventStreamTests<TAggregate, TAggregateId, TEventStream, TActor> : EventingTests<TAggregate, TAggregateId>
        where TAggregateId : IIdentity
        where TAggregate : IAggregate<TAggregateId>
        where TEventStream : IAsyncEventStream<TAggregate, TAggregateId>
        where TActor: IAsyncActor
    {
        protected IAsyncEventStream<TAggregate, TAggregateId> EventStream;
        
        protected IAsyncActor Actor;
        
        public EventStreamTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }
        
        [Fact]
        public void Needs_Actor()
        {
            Assert.NotNull(Actor);
        }


        [Fact]
        public void Needs_Connection()
        {
            Assert.NotNull(Connection);
        }
        
        public object Connection;

        [Fact]
        public void Needs_EventStream()
        {
            Assert.NotNull(EventStream);
        }


        [Fact]
        public void Must_EventStreamMustBeAssignableFromTEventStream()
        {
            Assert.IsAssignableFrom<TEventStream>(EventStream);
        }
        
        
        
    }
}