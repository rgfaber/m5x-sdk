using Xunit;
using Xunit.Abstractions;

namespace M5x.Testing.Tests
{
    public class ConsoleOutputTests : ConsoleOutputTestsBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ConsoleOutputTests(ITestOutputHelper output, ITestOutputHelper testOutputHelper)
            : base(output)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Try_WriteWelcome()
        {
            for (var j = 0; j <= 9; j++) _testOutputHelper.WriteLine($"Hello World {j}");
        }
    }
}