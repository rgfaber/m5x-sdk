using M5x.DEC.Schema;
using M5x.Testing;
using Microsoft.Extensions.Hosting;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration.Cmd
{
    public abstract class Emitter<TEmitter, TSubscriber, TAggregateId, TFact> : IoCTestsBase
        where TEmitter : IFactEmitter<TAggregateId, TFact>
        where TSubscriber : IHostedService
        where TAggregateId : IIdentity
        where TFact : IFact
    {
        protected TEmitter _emitter;
        protected TSubscriber _subscriber;

        protected Emitter(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        [Fact]
        public void Needs_Subscriber()
        {
            Assert.NotNull(_subscriber);
        }

        [Fact]
        public void Needs_Emitter()
        {
            Assert.NotNull(_emitter);
        }
    }
}