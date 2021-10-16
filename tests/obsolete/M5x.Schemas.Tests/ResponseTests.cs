using System.Threading.Tasks;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Schemas.Tests
{
    public class ResponseTests : IoCTestsBase
    {
        public ResponseTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
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
        }


        [Fact]
        public void Should_ResponseCreatedWithCorrelationIdHaveErrors()
        {
            var rsp = new TestResponse();
            Assert.NotNull(rsp.ErrorState);
        }


        private record TestResponse : Response
        {
        }
    }
}