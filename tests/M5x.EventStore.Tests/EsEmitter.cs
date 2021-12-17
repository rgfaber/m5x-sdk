using System.Collections.Generic;
using System.Threading.Tasks;
using EventStore.Client;
using M5x.EventStore.Interfaces;

namespace M5x.EventStore.Tests;

public interface IEsEmitter
{
    Task<IWriteResult> EmitAsync(string streamName, IEnumerable<EventData> events);
}

internal class EsEmitter : IEsEmitter
{
    private readonly IEsClient _client;

    public EsEmitter(IEsClient client)
    {
        _client = client;
    }

    public Task<IWriteResult> EmitAsync(string streamName, IEnumerable<EventData> events)
    {
        return _client.AppendToStreamAsync(streamName, StreamState.Any, events);
    }
}