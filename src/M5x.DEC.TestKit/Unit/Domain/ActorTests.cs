using System.Threading.Tasks;
using M5x.DEC.Commands;
using M5x.DEC.PubSub;
using M5x.DEC.Schema;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Unit.Domain
{
    public abstract class ActorTests<TActor, TAggregate, TId, TCommand, TFeedback> 
        : IoCTestsBase
        where TActor : IAsyncActor<TId, TCommand, TFeedback>
        where TFeedback : IFeedback
        where TAggregate : IAggregateRoot
        where TId : IIdentity
        where TCommand : ICommand<TId>
    {
        protected IIdentity ID;
        protected IAsyncActor<TId,TCommand,TFeedback> Actor;
        protected ICommand Command;
        protected IFeedback Feedback;
        protected IAggregateRoot Aggregate;

        protected ActorTests(ITestOutputHelper output,
            IoCTestContainer container) : base(output, container)
        {
        }

        [Fact]
        public void Needs_DECBus()
        {
            Assert.NotNull(Bus);
        }

        protected IDECBus Bus;
        
        


        [Fact]
        public void Needs_Aggregate()
        {
            Assert.NotNull(Aggregate);
        }

        [Fact]
        public void Must_AggregateMustBeTypeTAggregate()
        {
            Assert.IsType<TAggregate>(Aggregate);
        }

        [Fact]
        public void Must_FeedbackMustTypeTFeedback()
        {
            Assert.IsType<TFeedback>(Feedback);
        }

        [Fact]
        public void Must_CommandMustBeTypeTCommand()
        {
            Assert.IsType<TCommand>(Command);
        }

        [Fact]
        public void Needs_ID()
        {
            Assert.NotNull(ID);
        }

        [Fact]
        public void Must_IDMustBeOfTypeTId()
        {
            Assert.IsType<TId>(ID);
        }
        

        [Fact]
        public void Must_ActorMustBeAssignableFromTActor()
        {
            Assert.IsAssignableFrom<TActor>(Actor);
        }


        [Fact]
        public void Needs_Feedback()
        {
            Assert.NotNull(Feedback);
        }


        [Fact]
        public void Needs_Actor()
        {
            Assert.NotNull(Actor);
        }


        [Fact]
        public void Needs_Command()
        {
            Assert.NotNull(Command);
        }


        [Fact]
        public async Task<TFeedback> Must_ActorMustProvideFeedback()
        {
            var feedback = await Actor.HandleAsync((TCommand)Command);
            Assert.NotNull(feedback);
            return feedback;
        }


        [Fact]
        public async Task<TFeedback> Should_ActorShouldProvideFeedbackForHopeWithSameCorrelationId()
        {
            var feedback = await Must_ActorMustProvideFeedback();
            Assert.Equal(Command.CorrelationId, feedback.CorrelationId);
            return feedback;
        }

        [Fact]
        public async Task Must_FeedbackMustBeOfCorrectType()
        {
            var feedback = await Must_ActorMustProvideFeedback();
            Assert.IsType<TFeedback>(feedback);
        }


        [Fact]
        public abstract Task Should_ActorMustReturnFeedbackWithCorrectMetaData();

        [Fact]
        public abstract Task Must_ActorMustReturnErrorFeedbackForBadHope();
    }
}