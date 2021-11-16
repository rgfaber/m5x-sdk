using M5x.DEC;
using M5x.DEC.Commands;
using M5x.DEC.Events;
using M5x.DEC.ExecutionResults;
using M5x.DEC.Schema;

namespace Robby.Game.Domain
{
    public static partial class Aggregate
    {
        public interface IRoot : IAggregateRoot<Schema.Game.ID>
        {
        }

        public partial class Root : AggregateRoot<Schema.Game.ID>, IRoot
        {
            public Schema.Game Model;

            public Root()
            {
                Model = Schema.Game.New(Identity<Schema.Game.ID>.New.Value);
            }

            private Root(Schema.Game.ID gameId) : base(gameId)
            {
            }

            public static Root New(Schema.Game.ID gameId)
            {
                return new(gameId);
            }
        }

        public abstract record Cmd<TPayload> : Command<Schema.Game.ID> 
        where TPayload: IPayload
        {
            protected Cmd()
            {
            }

            protected Cmd(Schema.Game.ID aggregateId) : base(aggregateId)
            {
            }

            protected Cmd(Schema.Game.ID aggregateId, CommandId sourceId) : base(aggregateId, sourceId)
            {
            }

            protected Cmd(Schema.Game.ID aggregateId, string correlationId) : base(aggregateId, correlationId)
            {
            }


            protected Cmd(Schema.Game.ID aggregateId, TPayload payload) : base(aggregateId)
            {
                Payload = payload;
            }

            protected Cmd(Schema.Game.ID aggregateId, CommandId sourceId, TPayload payload) : base(aggregateId, sourceId)
            {
                Payload = payload;
            }

            protected Cmd(TPayload payload)
            {
                Payload = payload;
            }

            protected Cmd(Schema.Game.ID aggregateId, string correlationId, TPayload payload) : base(aggregateId, correlationId)
            {
                Payload = payload;
            }

            public TPayload Payload { get; set; }
            
        }

        public abstract record Evt<TPayload> : Event<Schema.Game.ID> 
            where TPayload : IPayload
        {
            protected Evt(TPayload payload)
            {
                Payload = payload;
            }

            protected Evt(AggregateInfo meta, string correlationId, TPayload payload) : base(meta, correlationId)
            {
                Payload = payload;
            }

            protected Evt(AggregateInfo meta, TPayload payload) : base(meta)
            {
                Payload = payload;
            }

            protected Evt()
            {
            }

            public TPayload Payload { get; set; }
            
        }
    }
}