using System;
using System.Linq;
using M5x.DEC.ExecutionResults;
using M5x.DEC.Schema;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Unit.Domain
{
    public abstract class AggregateTests<TAggregate, TAggregateId, TReadModel> : IoCTestsBase
        where TAggregate : IAggregateRoot<TAggregateId>
        where TAggregateId : IIdentity
        where TReadModel : IStateEntity<TAggregateId>
    {
        protected TAggregateId AggregateId;
        protected TReadModel StateModel;
        protected TAggregate Aggregate;


        public AggregateTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }


        [Fact]
        public void Needs_Aggregate()
        {
            Assert.NotNull(Aggregate);
        }

        [Fact]
        public void Needs_AggregateId()
        {
            Assert.NotNull(AggregateId);
        }

        [Fact]
        public void Needs_StateModel()
        {
            Assert.NotNull(StateModel);
        }


        protected void Given(params Func<TAggregate, bool>[] preConditions)
        {
            if (preConditions == null) Assert.True(true);
            var result = preConditions.Aggregate(true,
                (current,
                    preCondition) => current && preCondition.Invoke(Aggregate));
            Assert.True(result);
        }

        protected void When(params Func<TAggregate, IExecutionResult>[] executions)
        {
            var result = executions.Aggregate(true,
                (current,
                        execution) => current &&
                                      execution.Invoke(Aggregate)
                                          .IsSuccess);
            Assert.True(result);
        }


        protected void Then(params Func<TAggregate, bool>[] validations)
        {
            var result = validations.Aggregate(true,
                (current,
                    validation) => current && validation.Invoke(Aggregate));
            Assert.True(result);
        }


    }
}