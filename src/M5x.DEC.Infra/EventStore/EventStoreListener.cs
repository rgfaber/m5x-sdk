using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using M5x.DEC.Events;
using M5x.DEC.PubSub;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Utils;
using M5x.EventStore.Interfaces;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace M5x.DEC.Infra.EventStore;

public abstract class EventStoreListener<TAggregateId> : BackgroundService
    where TAggregateId : IIdentity
{
    private readonly IDECBus _bus;
    private readonly IEsClient _connection;
    private readonly IEnumerable<IEventHandler<TAggregateId, IEvent<TAggregateId>>> _handlers;
    private readonly ILogger _logger;
    private readonly bool _resolveLinkTos = true;
    private bool _isConnected;
    private string _logMessage;
    private Task<StreamSubscription> _subscription;

    protected EventStoreListener(
        IEsClient connection,
        IDECBus bus,
        IEnumerable<IEventHandler<TAggregateId, IEvent<TAggregateId>>> handlers,
        ILogger logger)
    {
        _logger = logger;
        _connection = connection;
        _bus = bus;
        _handlers = handlers;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
        }

        return Task.CompletedTask;
    }


    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _bus.Subscribe<IEvent<TAggregateId>>(async @event => await HandleAsync(_handlers, @event));
        return StartListening(cancellationToken);
    }

    private Task HandleAsync(IEnumerable<IEventHandler<TAggregateId, IEvent<TAggregateId>>> handlers,
        IEvent<TAggregateId> @event)
    {
        foreach (var handler in handlers)
            handler.HandleAsync(@event);
        return Task.CompletedTask;
    }


    public override void Dispose()
    {
        _subscription?.Dispose();
        base.Dispose();
    }

    private Task StartListening(CancellationToken cancellationToken)
    {
        try
        {
            var request = string.Empty;
            _subscription = _connection.SubscribeToAllAsync(
                FromAll.Start, 
                EventAppeared,
                false,
                SubscriptionDropped,
                cancellationToken: cancellationToken, filterOptions:
                new SubscriptionFilterOptions(StreamFilter.Prefix(AttributeUtils.GetIdPrefix<TAggregateId>())));
        }
        catch (Exception e)
        {
            _logger?.Fatal($"::EXCEPTION {e.Message})");
            throw;
        }

        return Task.CompletedTask;
    }

    private void SubscriptionDropped(StreamSubscription sub, SubscriptionDroppedReason reason, Exception exception)
    {
    }

    private Task EventAppeared(StreamSubscription sub,
        ResolvedEvent evt,
        CancellationToken cancellationToken)
    {
        var e = SerializationHelper.Deserialize<TAggregateId>(evt);
        _logger?.Debug($"EventStore: Event[{e}] appeared");
        return _bus.PublishAsync(e);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return StopListeningAsync(cancellationToken);
    }


    private Task StopListeningAsync(CancellationToken cancellationToken)
    {
        _logger?.Debug("EventStore: Stopped Listening");
        return Task.CompletedTask;
    }
}