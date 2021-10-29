using M5x.DEC.Infra.EventStore;
using M5x.DEC.Schema;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration.Etl
{
    public abstract class PlayerTests<TAggregateId, TReadModel> : IoCTestsBase
        where TAggregateId : IIdentity
    {
        protected TReadModel ExpectedState;
        protected IEventStorePlayer<TAggregateId> Player;


        protected PlayerTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        [Fact]
        public void Needs_Player()
        {
            Assert.NotNull(Player);
        }

        [Fact]
        public void Needs_ExpectedState()
        {
            Assert.NotNull(ExpectedState);
        }
    }
}