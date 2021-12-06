using M5x.DEC.Schema;
using Robby.Game.Schema;


namespace Robby.Game.Contract.Features
{
    public static class Start
    {

        [Topic(Config.Hopes.Start)]
        public record Hope : Hope<StartOrder> 
        {
            public Hope()
            {
            }

            public Hope(string aggregateId, string correlationId) : base(aggregateId, correlationId)
            {
            }

            public Hope(string aggregateId,
                string correlationId,
                StartOrder payload) : base(aggregateId,
                correlationId,
                payload)
            {
            }
        }

        [Topic(Config.Facts.Started)]
        public record Fact : Fact<StartOrder>
        {

            public Fact(AggregateInfo meta, string correlationId, StartOrder payload) 
                : base(meta, correlationId, payload)
            {
            }

            public static Fact New(AggregateInfo eventMeta, string eventCorrelationId, StartOrder eventPayload)
            {
                return new(eventMeta, eventCorrelationId, eventPayload);
            }
        }

        public record Fbk : Feedback<StartOrder>
        {
            public Fbk()
            {
            }

            public Fbk(AggregateInfo meta, string correlationId, StartOrder payload) 
                : base(meta, correlationId, payload)
            {
            }


            public static Fbk Empty(string correlationId)
            {
                return new Fbk(AggregateInfo.Empty, correlationId, StartOrder.Empty);
            }
        }
        
    }
}