using System;
using System.Linq;
using System.Threading.Tasks;
using M5x.DEC.Schema;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Unit.Domain;

public abstract class AggregateTests<TAggregate, TAggregateId, TReadModel> : IoCTestsBase
    where TAggregate : IAggregateRoot<TAggregateId>
    where TAggregateId : IIdentity
    where TReadModel : IStateEntity<TAggregateId>
{
    protected TAggregate Aggregate;
    protected TAggregateId AggregateId;
    protected TReadModel StateModel;


    public AggregateTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
    {
    }


    [Fact]
    public Task Needs_Aggregate()
    {
        Assert.NotNull(Aggregate);
        return Task.CompletedTask;
    }

    [Fact]
    public Task Needs_AggregateId()
    {
        Assert.NotNull(AggregateId);
        return Task.CompletedTask;
    }

    [Fact]
    public Task Needs_StateModel()
    {
        Assert.NotNull(StateModel);
        return Task.CompletedTask;
    }


    protected void Given(params Func<TAggregate, bool>[] preConditions)
    {
        if (preConditions == null)
            Assert.True(true);
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