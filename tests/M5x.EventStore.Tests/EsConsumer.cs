using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using EventStore.Client;
using M5x.EventStore.Interfaces;
using M5x.Testing;
using Microsoft.Extensions.Hosting;
using Xunit.Abstractions;

namespace M5x.EventStore.Tests;

public class EsConsumer : BackgroundService
{
    private readonly bool _autoAck = true;
    private readonly int _bufferSize = 10;
    private readonly IEsPersistentSubscriptionsClient _client;
    private readonly UserCredentials? _credentials;
    private readonly ITestOutputHelper _output;
    private readonly PersistentSubscriptionSettings _settings;
    private readonly TimeSpan? _deadline;
    private PersistentSubscription _subscription;

    public EsConsumer(IEsPersistentSubscriptionsClient client,
        PersistentSubscriptionSettings settings, ITestOutputHelper output)
    {
        _client = client;
        _settings = settings;
        _output = output;
        _deadline = null;
        _credentials = new UserCredentials(EventStoreConfig.UserName, EventStoreConfig.Password);
    }


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
        }

        return Task.CompletedTask;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        Guard.Against.Null(_settings, "PersistentSubscriptionSettings settings");
        try
        {
            try
            {
                await _client
                    .CreateAsync(TestConstants.Id,
                        TestConstants.GroupName,
                        _settings,
                        _deadline,
                        _credentials, 
                        cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _output.WriteLine($"{e.Message}");
            }

            _subscription = await _client.SubscribeAsync(
                    TestConstants.Id,
                    TestConstants.GroupName,
                    EventAppeared,
                    SubscriptionDropped,
                    _credentials,
                    _bufferSize,
                    cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _output.WriteLine($"{e}");
            throw;
        }
    }

    private Task EventAppeared(PersistentSubscription subscription, ResolvedEvent resolvedEvent, int? position,
        CancellationToken cancellationToken)
    {
        if (resolvedEvent.Event.EventType == TestData.MyFactType)
            _output?.WriteLine($"Event [{resolvedEvent.Event.EventType}] Appeared on \n" +
                               $"\tSubscription: {subscription.SubscriptionId}" +
                               $"\tGroup: {TestConstants.GroupName}\n" +
                               $"\tName: {TestConstants.Id}\n" +
                               $"\tPosition: {resolvedEvent.Event.Position}\n\n" +
                               $"{Serialize(resolvedEvent)} \n");
        return Task.CompletedTask;
    }



    private void SubscriptionDropped(PersistentSubscription subscription, SubscriptionDroppedReason droppedReason,
        Exception? exception)
    {
        _output?.WriteLine($"Subscription [{_subscription.SubscriptionId}] dropped. reason: {droppedReason}");
    }


    private string Serialize(ResolvedEvent resolvedEvent)
    {
        return JsonSerializer.Serialize(resolvedEvent);
    }
}