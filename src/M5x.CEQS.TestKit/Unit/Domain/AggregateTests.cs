using System;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Characteristics;
using EventFlow.Aggregates;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Core;
using EventFlow.ReadStores;
using M5x.Chassis.Compiler;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;
using IResolver = EventFlow.Configuration.IResolver;

namespace M5x.CEQS.TestKit.Unit.Domain
{
    public abstract class AggregateTests<TAggregate, TAggregateId, TReadModel> : IoCTestsBase
        where TAggregate : IAggregateRoot<TAggregateId>
        where TAggregateId : IIdentity
        where TReadModel: IReadModel
    {
        protected TAggregate Root;
        protected TAggregateId Id;
        protected TReadModel Model;
        protected IResolver Resolver;


        [Fact]
        public void Needs_Resolver()
        {
            Assert.NotNull(Resolver);
        }
        
        
        [Fact]
        public void Must_HaveRoot()
        {
            Assert.NotNull(Root);
        }
        
        [Fact]
        public void Must_HaveID()
        {
            Assert.NotNull(Id);
        }

        [Fact]
        public void Must_HaveModel()
        {
            Assert.NotNull(Model);
        }



        public AggregateTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }
        
        
        protected void Given(params Func<TAggregate, bool>[] preConditions)
        {
            var result = preConditions.Aggregate(true,
                (current,
                    preCondition) => current && preCondition.Invoke(Root));
            Assert.True(result);
        }

        protected void When(params Func<TAggregate, IExecutionResult>[] executions)
        {
            var result = true;
            foreach (var execution in executions)
            {
                result = result && execution.Invoke(Root).IsSuccess; ;
            }
            Assert.True(result);
        }
        
        
        protected void Then(params Func<TAggregate, bool>[] validations)
        {
            var result = validations.Aggregate(true, 
                (current, 
                    validation) => current && validation.Invoke(Root));
            Assert.True(result);
        }

        
        

        protected override async void Initialize()
        {
            Resolver = Container.GetService<IResolver>();
            Id = CreateId();
            Root = CreateAggregate();
            Model = CreateModel();
        }

        protected abstract TAggregateId CreateId();
        protected abstract TAggregate CreateAggregate();
        protected abstract TReadModel CreateModel();
    }
}