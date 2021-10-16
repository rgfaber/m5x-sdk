using System;
using System.Threading.Tasks;
using EventFlow.Aggregates;
using EventFlow.Commands;
using EventFlow.Configuration;
using EventFlow.Core;
using EventFlow.ReadStores;
using M5x.CEQS.Schema;
using M5x.CEQS.Schema.Utils;
using M5x.Testing;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace M5x.CEQS.TestKit.Unit.Domain
{
    public abstract class FeatureTests<TAggregate, TAggregateId, TReadModel, TCommand, TEvent, THope, TFeedback, TFact, THandler> :
        AggregateTests<TAggregate, TAggregateId, TReadModel> 
        where TAggregate : IAggregateRoot<TAggregateId>
        where TAggregateId : IIdentity
        where TReadModel : IReadModel
        where THope: IHope
        where TFeedback: IFeedback
        where TFact: IFact
        where THandler: ICommandHandler
    {
        protected FeatureTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        protected override async void Initialize()
        {
            try
            {
                base.Initialize();
                CorrelationId = GuidUtils.NewCleanGuid;
                Hope = CreateHope();
                Feedback = CreateFeedBack();
                Fact = CreateFact();
                Resolver = Container.GetService<IResolver>();
                Handler = CreateHandler();
                Cmd = CreateCmd();
                Evt = CreateEvt();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        protected abstract THandler CreateHandler();


        protected abstract TFact CreateFact();

        protected abstract TFeedback CreateFeedBack();

        protected abstract THope CreateHope();

        protected abstract TEvent CreateEvt();
        protected abstract TCommand CreateCmd();


        [Fact]
        public void Needs_CorrelationId()
        {
            Assert.NotNull(CorrelationId);
        }

        [Fact]
        public void Must_HaveCommand()
        {
            Assert.NotNull(Cmd);
        }

        [Fact]
        public void Needs_Hope()
        {
            Assert.NotNull(Hope);
        }

        [Fact]
        public void Needs_Feedback()
        {
            Assert.NotNull(Feedback);
        }

        [Fact]
        public void Needs_Fact()
        {
            Assert.NotNull(Fact);
        }
        
        

        [Fact]
        public void Must_HaveEvt()
        {
            Assert.NotNull(Evt);
        }


        [Fact]
        public void Needs_Handler()
        {
            Assert.NotNull(Handler);
        }


        protected THope Hope;
        protected TFeedback Feedback;
        protected TFact Fact;
        protected TCommand Cmd;
        protected TEvent Evt;
        protected THandler Handler;
        protected string CorrelationId;

    }
}