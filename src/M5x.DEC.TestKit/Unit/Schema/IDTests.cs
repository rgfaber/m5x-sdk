using M5x.DEC.Schema;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Unit.Schema
{
    public abstract class IDTests<TAggregateId> : IoCTestsBase where TAggregateId : IIdentity
    {
        protected IIdentity ID;

        public IDTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        [Fact]
        public void Must_HaveID()
        {
            Assert.NotNull(ID);
        }

        protected override void Initialize()
        {
            ID = NewID();
        }

        protected abstract TAggregateId NewID();
    }
}