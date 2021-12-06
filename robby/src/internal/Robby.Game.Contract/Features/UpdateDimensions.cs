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

        public record Fbk : Feedback
        {
            public Fbk()
            {
            }

            private Fbk(AggregateInfo meta, string correlationId) 
                : base(meta, correlationId)
            {
            }


            public static Fbk Empty(string cmdCorrelationId)
            {
                return new Fbk( AggregateInfo.Empty, cmdCorrelationId);
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