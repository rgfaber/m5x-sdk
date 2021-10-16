using M5x.DEC.Schema;
using M5x.DEC.Schema.Common;
using Robby.Game.Schema;

namespace Robby.Game.Contract.Features
{
    public static class UpdateDescription
    {
        [Topic(Config.Hopes.UpdateDescription)]
        public record Hope : Hope<Description>
        {
            public Hope(string aggregateId, string correlationId, Description payload) : base(aggregateId,
                correlationId)
            {
                Payload = payload;
            }

            public Hope()
            {
            }

            public Hope(string aggregateId, string correlationId) : base(aggregateId, correlationId)
            {
            }

            public static Hope New(string aggregateId, string correlationId, string name)
            {
                return new Hope(aggregateId, correlationId, Description.New(name));
            }
        }


        [Topic(Config.Facts.DescriptionUpdated)]
        public record Fact : Fact<Description>
        {

            public Fact(AggregateInfo meta, string correlationId, Description payload) 
                : base(meta, correlationId, payload)
            {
            }


            public Fact()
            {
            }

            public static Fact New(AggregateInfo meta, string correlationId, Description payload)
            {
                return new(meta, correlationId, payload);
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
                return new Feedback(cmdCorrelationId);
            }
        }
    }
}