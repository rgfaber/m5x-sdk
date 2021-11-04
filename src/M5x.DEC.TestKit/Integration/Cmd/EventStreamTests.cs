using System.Threading.Tasks;
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
        public Task Needs_Actor()
        {
            Assert.NotNull(Actor);
            return Task.CompletedTask;
        }


        [Fact]
        public Task Needs_Connection()
        {
            Assert.NotNull(Connection);
            return Task.CompletedTask;
        }
        
        public object Connection;

        [Fact]
        public Task Needs_EventStream()
        {
            Assert.NotNull(EventStream);
            return Task.CompletedTask;
        }


        [Fact]
        public Task Must_EventStreamMustBeAssignableFromTEventStream()
        {
            Assert.IsAssignableFrom<TEventStream>(EventStream);
            return Task.CompletedTask;
        }
        
        
        
    }
}