using System;
using System.Collections.Generic;
using System.Linq;
using M5x.DEC.PubSub;
using M5x.Schemas;
using M5x.Testing;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit
{
    
    
    
    
    public abstract class AggregateTests<TAggregate, TAggregateId>
        : IoCTestsBase
        where TAggregate : AggregateRoot<TAggregateId>
        where TAggregateId : IAggregateID
    {
        protected TAggregate _aggregateRoot;
        protected IAggregateSubscriber _subscriber;
        protected IAggregatePublisher _publisher;


        [Fact]
        public void Should_DomainMustContainAggregatePublisher()
        {
            Assert.NotNull(_publisher);
        }

        [Fact]
        public void Should_DomainMustContainAggregateSubscriber()
        {
            Assert.NotNull(_subscriber);
        }
        
        protected override void Initialize()
        {
            _aggregateRoot = GetAggregate();
            _publisher = Container.GetRequiredService<IAggregatePublisher>();
            _subscriber = Container.GetRequiredService<IAggregateSubscriber>();
        }

        protected abstract TAggregate GetAggregate();


        protected AggregateTests(ITestOutputHelper output, IoCTestContainer container) : base(
            output, container)
        {
        }

        protected void Given(IEnumerable<IEvent<TAggregateId>> events)
        {
            if (events != null) _aggregateRoot.Load(events);
        }

        protected void When(Action<TAggregate> command)
        {
            command(_aggregateRoot);
        }

        protected void Then<TEvent>(params Action<TEvent>[] conditions)
        {
            var events = _aggregateRoot.GetUncommittedEvents();
            events.Count().ShouldBe(1);
            var evnt = events.First();
            evnt.ShouldBeOfType<TEvent>();
            if (conditions != null) ((TEvent) evnt).ShouldSatisfyAllConditions(conditions);
        }

        protected void Throws<TException>(Action<TAggregate> command, params Action<TException>[] conditions)
            where TException : Exception
        {
            var ex = Should.Throw<TException>(() => command(_aggregateRoot));
            if (conditions != null) ex.ShouldSatisfyAllConditions(conditions);
        }


    }
}