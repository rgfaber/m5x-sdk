using System;
using M5x.DEC.Schema;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.Tests
{
    [IDPrefix(Prefix)]
    public record TestID : Identity<TestID>
    {
        public const string Prefix = "my";

        public TestID(string value) : base(value)
        {
        }
    }

    public class IdentityTests : IoCTestsBase
    {
        public IdentityTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        [Fact]
        public void Should_IdentityMustGiveErrorIfEmpty()
        {
            try
            {
                var v = string.Empty;
                var id = TestID.With(v);
            }
            catch (Exception e)
            {
                Assert.True(true);
            }
        }

        [Fact]
        public void Should_ReturnValidIdentityWithAnyString()
        {
            try
            {
                var expected = "my-bf3b1680-3aef-07d8-834b-f2b521407a6b";
                var v = "Too Lazy to type a string";
                var id = TestID.FromAnyString(v);
                Assert.Equal(expected, id.Value);
            }
            catch (Exception e)
            {
                Assert.True(false);
            }
        }


        [Fact]
        public void Should_ReturnValidIdentityFromDecimal()
        {
            try
            {
                var expected = "my-7f502725-628c-0004-0000-000000000000";
                decimal v = 1234255477745445;
                var id = TestID.FromDecimal(v);
                Assert.Equal(expected, id.Value);
            }
            catch (Exception e)
            {
                Assert.True(false);
            }
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
    }
}