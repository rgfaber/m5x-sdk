using M5x.DEC.Persistence;
using M5x.DEC.Schema;
using M5x.Testing;
using Microsoft.Extensions.Hosting;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration.Etl
{
    public abstract class SubscriberTests<TSubscriber, TWriter, TAggregateId, TFact, TReadModel> : IoCTestsBase
        where TSubscriber : IHostedService
        where TWriter : IModelWriter<TAggregateId, TFact, TReadModel>
        where TAggregateId : IIdentity
        where TFact : IFact
        where TReadModel : IReadEntity
    {
        protected TSubscriber Subscriber;
        protected TWriter Writer;

        protected SubscriberTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }


        [Fact]
        public void Needs_Subscriber()
        {
            Assert.NotNull(Subscriber);
        }

        [Fact]
        public void Needs_Writer()
        {
            Assert.NotNull(Writer);
        }
    }
}