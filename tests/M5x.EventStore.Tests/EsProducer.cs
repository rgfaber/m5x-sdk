using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using JetBrains.Annotations;
using M5x.Testing;
using Microsoft.Extensions.Hosting;
using Serilog;
using Xunit.Abstractions;

namespace M5x.EventStore.Tests;

public class EsProducer : BackgroundService
{
    private readonly IEsEmitter _emitter;
    [CanBeNull] private readonly ILogger _logger;
    private readonly ITestOutputHelper _output;

    public EsProducer(IEsEmitter emitter, ITestOutputHelper output)
    {
        _emitter = emitter;
        _output = output;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var rev = StreamRevision.None;
        while (!cancellationToken.IsCancellationRequested)
        {
            var ev = TestData.EventData(Guid.NewGuid());
            var events = new[] { ev };
            Thread.Sleep(2 * 1000);
            var res = await _emitter.EmitAsync(TestConstants.Id, events);
            rev = res.NextExpectedStreamRevision;
            _output.WriteLine($"StreamRevision: {rev}");
            var pos = res.LogPosition;
            _output.WriteLine($"Position: {pos}");
        }
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
    }
}