using System;
using System.Collections.Generic;
using M5x.DEC.PubSub;
using M5x.Schemas;
using M5x.Schemas.Commands;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit
{
    public abstract class DomainTests<TAggregate, TId, TCommand, TEvent> : IoCTestsBase
        where TAggregate: IAggregateRoot<TId>
        where TId : IAggregateID
        where TCommand : ICommand<TId>
        where TEvent : IEvent<TId>
    {
        private IEnumerable<TEvent> _inputEvents;

        private TAggregate _root;
        private IAggregatePublisher _publisher;
        private IAggregateSubscriber _subscriber;

        public DomainTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        private void Given(params Func<IAggregateRoot, bool>[] preConditions)
        {
            Assert.True(_root.ValidateState(preConditions));
        }

        private void When(Func<TAggregate, bool> execute)
        {
            Assert.True(execute.Invoke(_root));
        }

        private void Then(params Func<IAggregateRoot, bool>[] conditions)
        {
            foreach (var condition in conditions)
            {
                var validation = _root.ValidateState(condition);
                Assert.True(validation);
            }
        }

        private void Given(params Func<TAggregate, bool>[] executions)
        {
            if (executions == null) Assert.True(true);
            if (executions == null) return;
            foreach (var execution in executions)
                Assert.True(execution.Invoke(_root));
        }


        protected override void Initialize()
        {
            _publisher = Container.GetRequiredService<IAggregatePublisher>();
            _subscriber = Container.GetRequiredService<IAggregateSubscriber>();
            _root = Container.GetRequiredService<TAggregate>();
        }

        protected override void SetTestEnvironment()
        {
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
            services
                .AddDEC()
                .AddSingleton(x => GetAggregate());
        }

        protected abstract IAggregateRoot<TId> GetAggregate();
    }
}