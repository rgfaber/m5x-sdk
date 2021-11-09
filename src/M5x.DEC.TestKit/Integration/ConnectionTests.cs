using System.Threading.Tasks;
using M5x.Testing;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration
{
    public abstract class ConnectionTests<TConnection> : IoCTestsBase
    {
        protected object Connection;

        [Fact]
        public Task Needs_Connection()
        {
            Assert.NotNull(Connection);
            return Task.CompletedTask;
        }


        [Fact]
        public Task Must_ConnectionBeOfTypeTConnection()
        {
            Connection.ShouldBeAssignableTo<TConnection>();
            return Task.CompletedTask;
        }


        protected ConnectionTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

    }
}
