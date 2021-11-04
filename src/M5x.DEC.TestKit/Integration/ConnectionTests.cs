using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration
{
    public abstract class ConnectionTests<TConnection> : IoCTestsBase
    {
        protected object Connection;

        [Fact]
        public void Needs_Connection()
        {
            Assert.NotNull(Connection);
        }


        [Fact]
        public void Must_ConnectionMustBeAssignableFromTConnection()
        {
            Assert.IsAssignableFrom<TConnection>(Connection);
        }


        protected ConnectionTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

    }
}
