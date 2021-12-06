using M5x.DEC.Schema;
using M5x.DEC.Schema.Common;
using Robby.Game.Schema;


namespace Robby.Game.Contract.Features
{
    public static class Initialize
    {
        [Topic(Config.Facts.Initialized)]
        public record Fact : Fact<InitializationOrder>
        {
            public Fact() {}
            
            private Fact(AggregateInfo meta, string correlationId, InitializationOrder payload) : base(meta, correlationId,payload)
            {
            }
            public static Fact New(AggregateInfo meta, string correlationId, InitializationOrder payload)
            {
                return new(meta, correlationId, payload);
            }
        }

        [Topic(Config.Hopes.Initialize)]
        public record Hope : Hope<InitializationOrder>
        {
            public Hope()
            {
            }

            private Hope(string aggregateId, string correlationId, InitializationOrder payload) : base(aggregateId,
                correlationId)
            {
                Payload = payload;
            }

            public Hope(string aggregateId, string correlationId) : base(aggregateId, correlationId)
            {
            }

            public static Hope New(string correlationId, InitializationOrder payload)
            {
                return new(payload.AggregateId, correlationId, payload);
            }
        }

        public record Fbk : Feedback
        {
            public Fbk()
            {
            }


            private Fbk(AggregateInfo meta, string correlationId) : base(meta, correlationId)
            {
            }

            public static Fbk Empty(string correlationId)
            {
                return new(AggregateInfo.Empty, correlationId);
            }
        }
    }
}