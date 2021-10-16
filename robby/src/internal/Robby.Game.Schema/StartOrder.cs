using System;
using M5x.DEC.Schema;

namespace Robby.Game.Schema
{
    public record StartOrder: IPayload
    {
        public record Parameters
        {
            public Parameters()
            {
            }

            public Parameters(long numberOfMoves)
            {
                NumberOfMoves = numberOfMoves;
            }

            public long NumberOfMoves { get; set; }
        }

        public StartOrder(DateTime issuedAt, string gameId, Parameters settings)
        {
            IssuedAt = issuedAt;
            GameId = gameId;
            Settings = settings;
        }

        public StartOrder()
        {
        }

        public DateTime IssuedAt { get; set; }
        public string GameId { get; set; }
        public Parameters Settings { get; set; } 
    }


}