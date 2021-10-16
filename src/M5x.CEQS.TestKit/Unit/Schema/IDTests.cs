using System.Threading.Tasks;
using EventFlow.Core;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.CEQS.TestKit.Unit.Schema
{
    public abstract class IDTests<TAggregateId> : IoCTestsBase where TAggregateId: IIdentity
    {
        public IDTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }
        private IIdentity _id;
        [Fact]
        public void Must_HaveID()
        {
            Assert.NotNull(_id);
        }
        protected override async void Initialize()
        {
            _id = CreateNewID();
        }

        protected abstract TAggregateId CreateNewID();

    }
}