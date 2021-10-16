using System;
using System.Collections.Generic;
using M5x.DEC.Commands;
using M5x.DEC.Events;
using M5x.DEC.PubSub;
using M5x.DEC.Schema;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Unit.Domain
{
    public abstract class DomainTests<TAggregate, TId, TCommand, TEvent> : IoCTestsBase
        where TAggregate : IAggregateRoot<TId>
        where TId : IIdentity
        where TCommand : ICommand<TId>
        where TEvent : IEvent<TId>
    {
        protected IDECBus Bus;
        
        protected IEnumerable<TEvent> _inputEvents;

        protected TAggregate Aggregate;

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
        
        public DomainTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        private void Given(params Func<IAggregateRoot, bool>[] preConditions)
        {
            Assert.True(Aggregate.ValidateState(preConditions));
        }

        private void When(Func<TAggregate, bool> execute)
        {
            Assert.True(execute.Invoke(Aggregate));
        }

        private void Then(params Func<IAggregateRoot, bool>[] conditions)
        {
            foreach (var condition in conditions)
            {
                var validation = Aggregate.ValidateState(condition);
                Assert.True(validation);
            }
        }

        private void Given(params Func<TAggregate, bool>[] executions)
        {
            if (executions == null) Assert.True(true);
            if (executions == null) return;
            foreach (var execution in executions)
                Assert.True(execution.Invoke(Aggregate));
        }

 
    }
}