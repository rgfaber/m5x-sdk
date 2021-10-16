using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Robby.Schema.UnitTests
{
    public class RobotTests : IoCTestsBase
    {
        private Robot _robot;


        public RobotTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }


        [Fact]
        public void Should_RobotMustHaveAnId()
        {
            Assert.False(string.IsNullOrWhiteSpace(_robot.Id));
        }

        protected override async void Initialize()
        {
            _robot = new Robot();
        }

        protected override void SetTestEnvironment()
        {
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
        }
    }
}