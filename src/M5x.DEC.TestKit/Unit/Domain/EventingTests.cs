using System;
using System.Linq;
using M5x.DEC.Events;
using M5x.DEC.PubSub;
using M5x.DEC.Schema;
using M5x.Testing;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Unit.Domain
{
    public abstract class EventingTests<TAggregate, TAggregateId>
        : IoCTestsBase
        where TAggregate : AggregateRoot<TAggregateId>
        where TAggregateId : IIdentity
    {
        protected TAggregate Aggregate;

        protected IDECBus Bus;


        protected EventingTests(ITestOutputHelper output, IoCTestContainer container) : base(
            output, container)
        {
        }


        [Fact]
        public void Needs_Aggregate()
        {
            Assert.NotNull(Aggregate);
        }


        [Fact]
        public void Needs_DECBus()
        {
            Assert.NotNull(Bus);
        }
        
        protected void GivenInputEvents(params IEvent<TAggregateId>[] events)
        {
            if (events != null)
                Aggregate.Load(events);
        }

        protected void WhenCommand(Action<TAggregate> command)
        {
            command(Aggregate);
        }

        protected void ThenOutputEvent<TEvent>(params Action<TEvent>[] conditions)
        {
            var events = Aggregate.GetUncommittedEvents().ToList();
            Assert.NotNull(events);
            Assert.True(events.Any());
            var tEvents = events.OfType<TEvent>();
            foreach (var evnt in tEvents)
            {
                evnt.ShouldBeOfType<TEvent>();
                evnt.ShouldSatisfyAllConditions(conditions);
            }
        }

        protected void Throws<TException>(Action<TAggregate> command, params Action<TException>[] conditions)
            where TException : Exception
        {
            var ex = Should.Throw<TException>(() => command(Aggregate));
            if (conditions != null) ex.ShouldSatisfyAllConditions(conditions);
        }
    }
}