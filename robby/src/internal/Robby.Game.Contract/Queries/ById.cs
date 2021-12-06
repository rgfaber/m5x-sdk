using System.Collections.Generic;
using M5x.DEC.Schema;
using Robby.Game.Schema;

namespace Robby.Game.Contract.Queries
{
    public static class ById
    {
        public record Qry : Query
        {
            public string Id { get; set; }
            
            public Qry()
            {
            }

            public Qry(string correlationId) : base(correlationId)
            {
            }
        }

        public record Rsp : MultiResponse<Schema.GameModel>
        {
            public Rsp(string correlationId, IEnumerable<GameModel> payload) : base(correlationId, payload)
            {
            }

            public Rsp()
            {
            }

            public Rsp(IEnumerable<GameModel> payload) : base(payload)
            {
            }

            public static Rsp New(Qry qry, IEnumerable<GameModel> payload)
            {
                return new(qry.CorrelationId, payload);
            }
        }
    }
}