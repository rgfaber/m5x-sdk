using M5x.DEC.PubSub;
using M5x.DEC.TestKit.Tests.SuT;
using M5x.DEC.TestKit.Unit.Domain;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Tests
{
    public class MyEventingTests : EventingTests<MyAggregate, MyID>
    {
        public MyEventingTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        protected override void Initialize()
        {
            Bus = Container.GetRequiredService<IDECBus>();
            Aggregate = MyTestDomain.Aggregate;
            ID = MyID.New;
            Bus = Container.GetRequiredService<IDECBus>();
        }

        protected override void SetTestEnvironment()
        {
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
            services.AddDECBus();
        }

        [Fact]
        public void Should_ValidMyCommandShouldTriggerMyEvent()
        {
            GivenInputEvents();
            WhenCommand(agg => agg.Execute(MyTestDomain.Command));
            ThenOutputEvent<MyEvent>(evt => { evt.Payload.ShouldBeEquivalentTo(MyTestDomain.Command.Payload); });
        }
    }
}