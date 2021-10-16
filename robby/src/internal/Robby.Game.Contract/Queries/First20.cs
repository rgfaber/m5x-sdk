using System.Collections.Generic;
using M5x.DEC.Schema;

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

        public record Rsp : Response<IEnumerable<Game.Schema.Game>>
        {
            public Rsp()
            {
            }

            private Rsp(string correlationId) : base(correlationId)
            {
            }

            public Rsp(IEnumerable<Schema.Game> data) : base(data)
            {
            }

            public Rsp(string correlationId, IEnumerable<Schema.Game> data) : base(correlationId, data)
            {
            }

            public static Rsp New(Qry qry)
            {
                return new(qry.CorrelationId);
            }
        }
    }
}