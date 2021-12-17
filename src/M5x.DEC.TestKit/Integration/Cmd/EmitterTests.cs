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

namespace M5x.DEC.TestKit.Integration.Cmd;

public abstract class EmitterTests<
    TConnection,
    TEmitter,
    TSubscriber,
    TAggregateId,
    TEvent,
    TFact> : ConnectedTests<TConnection>
    where TEmitter : IFactEmitter<TAggregateId, TEvent, TFact>
    where TAggregateId : IIdentity
    where TFact : IFact
    where TEvent : IEvent<TAggregateId>
    where TSubscriber : ISubscriber<TAggregateId, TFact>
{
    protected IDECBus Bus;

//        protected IFactEmitter<TAggregateId,TEvent,TFact> Emitter;
    protected object Emitter;
    protected IFactHandler<TAggregateId, TFact> FactHandler;
    protected ILogger Logger;
    protected ISubscriber<TAggregateId, TFact> Subscriber;


    public EmitterTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
    {
    }

    [Fact]
    public Task Needs_DECBus()
    {
        Assert.NotNull(Bus);
        return Task.CompletedTask;
    }

    [Fact]
    public Task Needs_OutFact()
    {
        Assert.NotNull(TestFacts.OutFact);
        return Task.CompletedTask;
    }

    [Fact]
    public Task Needs_Emitter()
    {
        Assert.NotNull(Emitter);
        return Task.CompletedTask;
    }

    [Fact]
    public Task Needs_Subscriber()
    {
        Assert.NotNull(Subscriber);
        return Task.CompletedTask;
    }

    [Fact]
    public Task Needs_Logger()
    {
        Assert.NotNull(Logger);
        return Task.CompletedTask;
    }

    [Fact]
    public Task Needs_FactHandler()
    {
        Assert.NotNull(FactHandler);
        return Task.CompletedTask;
    }

    [Fact]
    public Task Must_EmitterMustHaveTopic()
    {
        Assert.False(string.IsNullOrWhiteSpace(((TEmitter)Emitter).Topic));
        return Task.CompletedTask;
    }

    [Fact]
    public Task Must_EmitterMustHaveFactTopic()
    {
        Assert.Equal(
            ((IFactEmitter<TAggregateId, TEvent, TFact>)Emitter).Topic,
            AttributeUtils.GetTopic<TFact>());
        return Task.CompletedTask;
    }

    [Fact]
    public Task Needs_OutEvent()
    {
        Assert.NotNull(TestEvents.OutEvent);
        return Task.CompletedTask;
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
            await ((IFactEmitter<TAggregateId, TEvent, TFact>)Emitter).HandleAsync(TestEvents.OutEvent)
                .ConfigureAwait(false);
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