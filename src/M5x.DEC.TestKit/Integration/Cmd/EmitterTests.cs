using System;
using System.Threading;
using System.Threading.Tasks;
using M5x.DEC.Events;
using M5x.DEC.Persistence;
using M5x.DEC.PubSub;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using M5x.DEC.Schema.Utils;
using M5x.Testing;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration.Cmd
{
    public abstract class EmitterTests<
        TEmitter,
        TSubscriber,
        TAggregateId,
        TEvent,
        TFact> : ConnectedTests
        where TEmitter : IFactEmitter<TAggregateId,TEvent,TFact>
        where TAggregateId : IIdentity
        where TFact : IFact
        where TEvent : IEvent<TAggregateId>
        where TSubscriber : ISubscriber<TAggregateId, TFact>
    {
        protected IDECBus Bus;
        protected IFactEmitter<TAggregateId,TEvent,TFact> Emitter;
        protected IFactHandler<TAggregateId, TFact> FactHandler;
        protected ILogger Logger;
        protected ISubscriber<TAggregateId,TFact> Subscriber;
        


        public EmitterTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }
        
        [Fact]
        public void Needs_DECBus()
        {
            Assert.NotNull(Bus);
        }

        [Fact]
        public void Needs_OutFact()
        {
            Assert.NotNull(TestFacts.OutFact);
        }

        [Fact]
        public void Needs_Emitter()
        {
            Assert.NotNull(Emitter);
        }

        [Fact]
        public void Needs_Subscriber()
        {
            Assert.NotNull(Subscriber);
        }

        [Fact]
        public void Needs_Logger()
        {
            Assert.NotNull(Logger);
        }

        [Fact]
        public void Needs_FactHandler()
        {
            Assert.NotNull(FactHandler);
        }

        [Fact]
        public void Must_EmitterMustHaveTopic()
        {
            Assert.False(string.IsNullOrWhiteSpace(Emitter.Topic));
        }

        [Fact]
        public void Must_EmitterMustHaveFactTopic()
        {
            Assert.Equal(Emitter.Topic, AttributeUtils.GetTopic<TFact>());
        }

        [Fact]
        public void Needs_OutEvent()
        {
            Assert.NotNull(TestEvents.OutEvent);
        }

        [Fact]
        public async Task Must_EmitterMustEmitFact()
        {
            Assert.NotNull(TestFacts.OutFact);
            Assert.NotNull(Emitter);
            var subHost = Container.GetRequiredService<IHostExecutor>();
            var cs = new CancellationTokenSource();
            Assert.NotNull(subHost);
            try
            {
                await subHost.StartAsync(cs.Token);
                Output?.WriteLine($"Emitting Fact: {TestFacts.OutFact}");
                await Emitter.HandleAsync(TestEvents.OutEvent, cs.Token).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Output?.WriteLine(e.InnerAndOuter());
                throw;
            }
            finally
            {
                cs.Cancel();
                await subHost.StopAsync(cs.Token).ConfigureAwait(false);
            }
        }
        
        public class TheFactHandler : IFactHandler<TAggregateId, TFact>
        {
            public Task HandleAsync(TFact fact)
            {
                TestFacts.InFact = fact;
                Assert.True(TestFacts.OutFact.CorrelationId == TestFacts.InFact.CorrelationId);
                return Task.CompletedTask;
            }
        }

        public static class TestFacts
        {
            public static TFact InFact;
            public static TFact OutFact;
        }

        public static class TestEvents
        {
            public static TEvent OutEvent;
        }
    }
}