﻿namespace M5x.DEC.Sagas
{
    public static class MetadataKeys
    {
        public const string EventId = "event_id";
        public const string BatchId = "batch_id";
        public const string EventName = "event_name";
        public const string EventVersion = "event_version";
        public const string Timestamp = "timestamp";
        public const string TimestampEpoch = "timestamp_epoch";
        public const string AggregateSequenceNumber = "aggregate_sequence_number";
        public const string AggregateName = "aggregate_name";
        public const string AggregateId = "aggregate_id";
        public const string SourceId = "source_id";
        public const string CorrelationId = "correlation_id";
        public const string CausationId = "causation_id";
    }
}