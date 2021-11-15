using System.Threading.Tasks;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration
{
    public abstract class ConnectionTests<TConnection> : IoCTestsBase
    {
        protected object Connection;


        protected ConnectionTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        [Fact]
        public Task Needs_Connection()
        {
            Assert.NotNull(Connection);
            return Task.CompletedTask;
        }


        [Fact]
        public Task Must_ConnectionBeOfTypeTConnection()
        {
            Assert.IsAssignableFrom<TConnection>(Connection);
//            Connection.ShouldBeAssignableTo<TConnection>();
            return Task.CompletedTask;
        }
    }
}