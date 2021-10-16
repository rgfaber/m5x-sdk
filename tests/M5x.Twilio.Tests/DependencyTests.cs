using System.Threading.Tasks;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Twilio.Tests
{
    public class DependencyTests : IoCTestsBase
    {
        public DependencyTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        protected override void Initialize()
        {
        }

        protected override void SetTestEnvironment()
        {
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
            services.AddTwilio();
        }

        [Fact]
        public void Must_Contain_TwilioFactory()
        {
            var fact = Container.GetService<ITwilioFactory>();
            Assert.NotNull(fact);
        }
    }
}