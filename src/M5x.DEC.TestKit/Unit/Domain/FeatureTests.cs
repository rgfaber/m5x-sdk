using System;
using System.Text.Json;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Utils;
using M5x.Testing;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Unit.Domain
{
    public abstract class FeatureTests<TAggregate, TAggregateId, TReadModel, TCommand, TEvent, THope, TFeedback,
        TFact> : AggregateTests<TAggregate, TAggregateId, TReadModel>
        where TAggregate : IAggregateRoot<TAggregateId>
        where TAggregateId : IIdentity
        where TReadModel : IStateEntity<TAggregateId>
        where THope : IHope
        where TFeedback : IFeedback
        where TFact : IFact
    {
        protected TCommand Command;
        protected string CorrelationId;
        protected TEvent Event;
        protected TFact Fact;
        protected TFeedback Feedback;
        protected THope Hope;


        protected FeatureTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }



        [Fact]
        public void Needs_CorrelationId()
        {
            Assert.NotNull(CorrelationId);
        }

        [Fact]
        public void Needs_Command()
        {
            Assert.NotNull(Command);
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
        public void Needs_Event()
        {
            Assert.NotNull(Event);
        }
        
        [Fact]
        public void Must_EventMustBeDeserializable()
        {
            var s = JsonSerializer.SerializeToUtf8Bytes(Event);
            var res = JsonSerializer.Deserialize<TEvent>(s);
            Assert.NotNull(res);
        }
        
        
        
    }
}