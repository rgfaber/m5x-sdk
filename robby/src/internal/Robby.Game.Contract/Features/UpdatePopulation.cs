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

            public static Hope New(string id, string correlationId, int numberOfRobots)
            {
                throw new System.NotImplementedException();
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

            public static Feedback Empty(string correlationId)
            {
                return new(correlationId);
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