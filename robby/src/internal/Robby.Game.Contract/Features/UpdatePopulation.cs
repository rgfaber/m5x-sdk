using M5x.DEC.Schema;
using M5x.DEC.Schema.Common;
using Robby.Game.Schema;

namespace Robby.Game.Contract.Features
{
    public static class UpdatePopulation
    {
        [Topic(Config.Hopes.UpdatePopulation)]
        public record Hope : Hope<Population>
        {
            public Hope()
            {
            }

            private Hope(string aggregateId,
                string correlationId,
                Population payload) : base(aggregateId,
                correlationId,
                payload)
            {
            }

            public static Hope New(string id, string correlationId, Population payload)
            {
                return new Hope(id, correlationId, payload);
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


            public static Fbk Empty(string correlationId)
            {
                return new(AggregateInfo.Empty,  correlationId);
            }
        }

        [Topic(Config.Facts.PopulationUpdated)]
        public record Fact : Fact<Population>
        {

            public Fact(AggregateInfo meta, string correlationId, Population payload) 
                : base(meta, correlationId, payload)
            {
            }


            public Fact()
            {
            }

            public static Fact New(AggregateInfo meta, string correlationId, Population payload)
            {
                return new(meta, correlationId, payload);
            }
        }
    }
}