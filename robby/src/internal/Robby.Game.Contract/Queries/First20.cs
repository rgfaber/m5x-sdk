using System.Collections.Generic;
using M5x.DEC.Schema;
using Robby.Game.Schema;

namespace Robby.Game.Contract.Queries
{
    public static class First20
    {
        public record Qry : Query
        {
            private Qry(string correlationId) : base(correlationId)
            {
            }

            public Qry()
            {
            }

            public static Qry New(string correlationId)
            {
                return new(correlationId);
            }
        }

        public record Rsp : MultiResponse<Game.Schema.GameModel>
        {
            public Rsp()
            {
            }

            public Rsp(IEnumerable<GameModel> data) : base(data)
            {
            }

            public Rsp(string correlationId, IEnumerable<Schema.GameModel> data) : base(correlationId, data)
            {
            }

            public static Rsp New(string correlationId, IEnumerable<GameModel> payload)
            {
                return new(correlationId, payload);
            }
        }
    }
}