using System;
using System.Text.Json;
using M5x.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Tests
{
    public class ExtensionTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ExtensionTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }


        [Fact]
        public void Try_DivideByZero()
        {
            try
            {
                var zero = 1 - 1;
                var t = 1 / zero;
            }
            catch (Exception e)
            {
                var xp = e.ToXeption();
                _testOutputHelper.WriteLine(JsonSerializer.Serialize(xp));
                throw;
            }
        }
    }
}