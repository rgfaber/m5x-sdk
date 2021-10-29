using M5x.DEC.Events;
using M5x.DEC.Schema;
using M5x.DEC.TestKit.Integration.Cmd;
using M5x.Testing;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration.Client
{
    public abstract class SubscriberTests<
        TEmitter,
        TSubscriber,
        TAggregateId,
        TEvent,
        TFact> : EmitterTests<TEmitter, TSubscriber, TAggregateId, TEvent, TFact>
        where TEmitter : IFactEmitter<TAggregateId, TEvent, TFact>
        where TAggregateId : IIdentity
        where TFact : IFact
        where TEvent : IEvent<TAggregateId>
        where TSubscriber : ISubscriber<TAggregateId, TFact>
    {
        public SubscriberTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }
    }
}