using System.CodeDom;
using System.Threading.Tasks;
using FakeItEasy;
using M5x.Config;
using M5x.DEC.TestKit.Tests.SuT;
using M5x.DEC.TestKit.Unit.Domain;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute.Routing.Handlers;
using Shouldly;
using Xunit;
using Xunit.Abstractions;


namespace M5x.DEC.TestKit.Tests
{
    public class MyActorTests : ActorTests<IMyActor, MyAggregate, MyID, MyCommand, MyHope, MyFeedback>
    {
        public MyActorTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        protected override void Initialize()
        {
            Actor = Container.GetRequiredService<IMyActor>();
            Feedback = MyTestContract.Feedback;
            Hope = MyTestContract.Hope;
        }

        protected override void SetTestEnvironment()
        {
            DotEnv.FromEmbedded();
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
            services.AddMyFakeActor();
        }


        public override async Task Should_ActorMustReturnFeedbackWithCorrectMetaData()
        {
            var feedback = await Should_ActorShouldProvideFeedbackForHopeWithSameCorrelationId();
            Assert.NotNull(feedback);
        }

        public override async Task Must_ActorMustReturnErrorFeedbackForBadHope()
        {
            var hope = MyTestContract.Hope;
            hope.Payload = null;
            var res = await Actor.HandleAsync(hope).ConfigureAwait(false);
            res.IsSuccess.ShouldBe(false);
        }
        
        
        
        
        
    }
}