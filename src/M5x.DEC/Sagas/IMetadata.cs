using System;
using System.Collections.Generic;
using M5x.DEC.Commands;
using M5x.DEC.Core;

namespace M5x.DEC.Sagas
{
    public interface IMetadata : IMetadataContainer
    {
        IEventId EventId { get; }
        ISourceID SourceId { get; }
        string EventName { get; }
        int EventVersion { get; }
        DateTimeOffset Timestamp { get; }
        long TimestampEpoch { get; }
        long AggregateSequenceNumber { get; }
        string AggregateId { get; }
        string CorrelationId { get; }
        string CausationId { get; }

        IMetadata CloneWith(params KeyValuePair<string, string>[] keyValuePairs);
        IMetadata CloneWith(IEnumerable<KeyValuePair<string, string>> keyValuePairs);
    }
}