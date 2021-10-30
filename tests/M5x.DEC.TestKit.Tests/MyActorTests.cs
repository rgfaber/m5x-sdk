using System.CodeDom;
using System.Threading.Tasks;
using FakeItEasy;
using M5x.Config;
using M5x.DEC.PubSub;
using M5x.DEC.Schema.Common;
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
    public class MyActorTests : ActorTests<IMyActor, 
        MyAggregate, MyID, MyCommand, MyFeedback>
    {
        public MyActorTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        protected override void Initialize()
        {
            Actor = Container.GetRequiredService<IMyActor>();
            Feedback = MyTestContract.Feedback;
            Command = MyTestDomain.Command;
            ID = MyID.NewComb();
            Feedback = MyFeedback.New(TestConstants.Meta, TestConstants.CORRELATION_ID, Dummy.Empty);
            Aggregate = MyAggregate.New((MyID)ID, MyReadModel.New(ID.Value));
            Bus = Container.GetRequiredService<IDECBus>();
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
            var cmd = (MyCommand)Command;
            cmd.Payload = null;
            var res = await Actor.HandleAsync(cmd).ConfigureAwait(false);
            res.IsSuccess.ShouldBe(false);
        }
        
        
        
        
        
    }
}