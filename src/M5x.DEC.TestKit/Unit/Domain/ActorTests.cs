using System.Threading.Tasks;
using M5x.DEC.Commands;
using M5x.DEC.PubSub;
using M5x.DEC.Schema;
using M5x.Testing;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Unit.Domain
{
    public abstract class ActorTests<TActor, TAggregate, TId, TCommand, TFeedback, TBroadcaster> 
        : IoCTestsBase
        where TActor : IAsyncActor<TId, TCommand, TFeedback>
        where TFeedback : IFeedback
        where TAggregate : IAggregateRoot
        where TId : IIdentity
        where TCommand : ICommand<TId>
        where TBroadcaster: IBroadcaster<TId>

    {
        protected IIdentity ID;
//        protected IAsyncActor<TId,TCommand,TFeedback> Actor;
        protected object Actor;
        protected ICommand Command;
        protected IFeedback Feedback;
        protected IAggregateRoot Aggregate;

        protected ActorTests(ITestOutputHelper output,
            IoCTestContainer container) : base(output, container)
        {
        }
        
        
        [Fact]
        public Task Needs_Broadcaster()
        {
            Assert.NotNull(Caster);
            return Task.CompletedTask;
        }

        [Fact]
        public Task Must_CasterMustBeAssignableToTBroadcaster()
        {
            Caster.ShouldBeAssignableTo<TBroadcaster>();
            return Task.CompletedTask;
        }
       
        
        protected object Caster { get; set; }

        
        
        

        [Fact]
        public Task Needs_DECBus()
        {
            Assert.NotNull(Bus);
            return Task.CompletedTask;
        }

        protected IDECBus Bus;
        
        


        [Fact]
        public Task Needs_Aggregate()
        {
            Assert.NotNull(Aggregate);
            return Task.CompletedTask;
        }

        [Fact]
        public Task Must_AggregateMustBeTypeTAggregate()
        {
            Assert.IsType<TAggregate>(Aggregate);
            return Task.CompletedTask;
        }

        [Fact]
        public Task Must_FeedbackMustTypeTFeedback()
        {
            Assert.IsType<TFeedback>(Feedback);
            return Task.CompletedTask;
        }

        [Fact]
        public Task Must_CommandMustBeTypeTCommand()
        {
            Assert.IsType<TCommand>(Command);
            return Task.CompletedTask;
        }

        [Fact]
        public Task Needs_ID()
        {
            Assert.NotNull(ID);
            return Task.CompletedTask;
        }

        [Fact]
        public Task Must_IDMustBeOfTypeTId()
        {
            Assert.IsType<TId>(ID);
            return Task.CompletedTask;
        }
        

        [Fact]
        public Task Must_ActorMustBeAssignableFromTActor()
        {
            Actor.ShouldBeAssignableTo<TActor>();
            Assert.IsAssignableFrom<TActor>(Actor);
            return Task.CompletedTask;
        }


        [Fact]
        public Task Needs_Feedback()
        {
            Assert.NotNull(Feedback);
            return Task.CompletedTask;
        }


        [Fact]
        public Task Needs_Actor()
        {
            Assert.NotNull(Actor);
            return Task.CompletedTask;
        }


        [Fact]
        public Task Needs_Command()
        {
            Assert.NotNull(Command);
            return Task.CompletedTask;
        }


        [Fact]
        public async Task<TFeedback> Must_ActorMustProvideFeedback()
        {
            var feedback = await ((TActor)Actor).HandleAsync((TCommand)Command);
            Assert.NotNull(feedback);
            return feedback;
        }


        [Fact]
        public async Task<object> Should_ActorShouldProvideFeedbackForHopeWithSameCorrelationId()
        {
            var feedback = await Must_ActorMustProvideFeedback();
            Assert.Equal(Command.CorrelationId, feedback.CorrelationId);
            return feedback;
        }

        [Fact]
        public async Task Must_FeedbackMustBeOfCorrectType()
        {
            var feedback = await Must_ActorMustProvideFeedback();
            Assert.NotNull(feedback);
            feedback.ShouldBeAssignableTo<TFeedback>();
//            Assert.IsType<TFeedback>(feedback);
        }


        [Fact]
        public abstract Task Should_ActorMustReturnFeedbackWithCorrectMetaData();

        [Fact]
        public abstract Task Must_ActorMustReturnErrorFeedbackForBadHope();
    }
}