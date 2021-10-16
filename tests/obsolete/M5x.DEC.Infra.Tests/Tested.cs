using System;
using M5x.Schemas;

namespace M5x.DEC.Infra.Tests
{
    [Topic("tested")]
    public record Tested: IEvent<TestAggregateId>
    {
        
        
        public Tested() {}
        
        private Tested(TestPayload payload, string correlationId, Guid eventId, TestAggregateId aggregateId, long aggregateVersion)
        {
            Payload = payload;
            CorrelationId = correlationId;
            EventId = eventId;
            AggregateId = aggregateId;
            AggregateVersion = aggregateVersion;
        }
        public TestPayload Payload { get; set; }
        public string CorrelationId { get; set; }
        public Guid EventId { get; }
        public TestAggregateId AggregateId { get; }
        public long AggregateVersion { get; }

        public static Tested CreateNew(TestAggregateId id)
        {
            return new Tested(new TestPayload("Jack",
                    DateTime.Now,
                    25),
                GuidFactories.NewCleanGuid,
                Guid.NewGuid(),
                id,
                0);
        }
    }
}