using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration
{
    public abstract class ConnectedTests<TConnection> : ConnectionTests<TConnection>
    {
        

        [Fact]
        public void Needs_HostExecutor()
        {
            Assert.NotNull(Executor);
        }
        
        
        
        public IHostExecutor Executor { get; set; }


        public object Connection { get; set; }

        protected ConnectedTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }
    }
}