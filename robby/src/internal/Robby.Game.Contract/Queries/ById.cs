using System.Collections.Generic;
using M5x.DEC.Schema;

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

        public record Rsp : Response<IEnumerable<Schema.Game>>
        {
            public Rsp()
            {
                Data = new List<Schema.Game>();
            }

            private Rsp(string correlationId) : base(correlationId)
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