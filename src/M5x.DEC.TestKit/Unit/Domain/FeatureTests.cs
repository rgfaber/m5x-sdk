using System.Text.Json;
using System.Threading.Tasks;
using M5x.DEC.Schema;
using M5x.Testing;
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
        public Task Needs_CorrelationId()
        {
            Assert.NotNull(CorrelationId);
            return Task.CompletedTask;
        }

        [Fact]
        public Task Needs_Command()
        {
            Assert.NotNull(Command);
            return Task.CompletedTask;
        }

        [Fact]
        public Task Needs_Hope()
        {
            Assert.NotNull(Hope);
            return Task.CompletedTask;
        }

        [Fact]
        public Task Needs_Feedback()
        {
            Assert.NotNull(Feedback);
            return Task.CompletedTask;
        }

        [Fact]
        public Task Needs_Fact()
        {
            Assert.NotNull(Fact);
            return Task.CompletedTask;
        }

        [Fact]
        public Task Needs_Event()
        {
            Assert.NotNull(Event);
            return Task.CompletedTask;
        }

        [Fact]
        public Task Must_EventMustBeDeserializable()
        {
            var s = JsonSerializer.SerializeToUtf8Bytes(Event);
            var res = JsonSerializer.Deserialize<TEvent>(s);
            Assert.NotNull(res);
            return Task.CompletedTask;
        }
    }
}