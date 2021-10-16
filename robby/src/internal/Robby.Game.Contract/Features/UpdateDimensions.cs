using M5x.DEC.Schema;
using M5x.DEC.Schema.Common;

namespace Robby.Game.Contract.Features
{
    public static class UpdateDimensions
    {
        [Topic(Config.Hopes.UpdateDimensions)]
        public record Hope : Hope<Vector>
        {
            public Hope(string aggregateId, string correlationId, Vector payload) : base(aggregateId, correlationId)
            {
                Payload = payload;
            }

            public Hope()
            {
            }

            public Hope(string aggregateId, string correlationId) : base(aggregateId, correlationId)
            {
            }

            public static Hope New(string aggregateId, string correlationId, Vector dimensions)
            {
                return new(aggregateId, correlationId, dimensions);
            }
        }

        public record Feedback : Feedback<Dummy>
        {
            public Feedback()
            {
            }

            public Feedback(AggregateInfo meta, string correlationId, Dummy payload) 
                : base(meta, correlationId, payload)
            {
            }

            public Feedback(string correlationId) : base(correlationId)
            {
            }

            public static Feedback Empty(string cmdCorrelationId)
            {
                return new(cmdCorrelationId);
            }
        }

        [Topic(Config.Facts.DimensionsUpdated)]
        public record Fact : Fact<Vector>
        {

            private Fact(AggregateInfo meta, string correlationId, Vector payload) 
                : base(meta, correlationId, payload)
            {
            }

            public Fact()
            {
            }

            public static Fact New(AggregateInfo meta, string correlationId, Vector payload)
            {
                return new(meta, correlationId, payload);
            }
        }
    }
}