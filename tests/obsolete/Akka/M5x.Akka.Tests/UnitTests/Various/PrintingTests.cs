using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using M5x.Akka.Extensions;
using Xunit.Abstractions;

namespace M5x.Akka.Tests.UnitTests.Various
{
    public class PrintingTests: IoCTestsBase
    {
        public enum ColorType
        {
            Red,
            Orange,
            Yellow,
            Green,
            Blue,
            Indigo,
            Violet
        }

        class TestMe
        {
            public string Id { get; set; }
            public string DisplayName { get; set; }
            public int Length { get; set; }
            public ColorType Color { get; set; }
        }

        [Fact]
        public void Try_PrettyPrint()
        {
            var s = typeof(TestMe).PrettyPrint();
            Output.WriteLine(s);
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

        public PrintingTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }
    }
}