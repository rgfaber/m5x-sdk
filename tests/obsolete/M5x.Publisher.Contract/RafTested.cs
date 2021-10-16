using System;
using M5x.Schemas;

namespace M5x.Publisher.Contract
{
    [Topic("raf-tested")]
    public record RafTested : Event<RafId>
    {
        public string Name { get; set; }
        public DateTime TestedAt { get; set; }
        
        public RafTested()
        {
        }

        public RafTested(RafId aggregateId) : base(aggregateId)
        {
        }

        public RafTested(RafId aggregateId, long aggregateVersion) : base(aggregateId, aggregateVersion)
        {
        }

        public override IEvent<RafId> WithAggregate(RafId aggregateId, long version)
        {
            return CreateNew(aggregateId, version);
        }

        public static RafTested CreateNew(RafId aggregateId, long version)
        {
            return new(aggregateId, version);
        }
    }
}