using System.Threading.Tasks;
using M5x.DEC.Commands;
using M5x.DEC.ExecutionResults;
using M5x.DEC.Schema;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Unit.Domain
{
        public abstract class ActorTests<TActor, TAggregate, TAggregateId, TCommand, THope, TFeedback>: IoCTestsBase
        where TActor: IAsyncActor<TAggregate, TAggregateId, TCommand, THope, TFeedback>
        where THope: IHope
        where TFeedback: IFeedback
        where TAggregate : IAggregateRoot<TAggregateId>
        where TAggregateId : IIdentity
        where TCommand : ICommand<TAggregate, TAggregateId, IExecutionResult>
        {
            protected TActor Actor;
            protected THope Hope;
            protected TFeedback Feedback;
            
            protected ActorTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
            {
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
            public void Needs_Hope()
            {
                Assert.NotNull(Hope);
            }


            [Fact]
            public async Task<TFeedback> Must_ActorMustProvideFeedback()
            {
                var feedback = await Actor.HandleAsync(Hope);
                Assert.NotNull(feedback);
                return feedback;
            }


            [Fact]
            public async Task<TFeedback> Should_ActorShouldProvideFeedbackForHopeWithSameCorrelationId()
            {
                var feedback = await Must_ActorMustProvideFeedback();
                Assert.Equal(Hope.CorrelationId, feedback.CorrelationId);
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