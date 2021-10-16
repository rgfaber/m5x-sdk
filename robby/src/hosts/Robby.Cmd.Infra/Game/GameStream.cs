using M5x.DEC.Persistence;
using M5x.DEC.Persistence.EventStore;
using M5x.DEC.PubSub;
using Robby.Domain.Game;

namespace Robby.Cmd.Infra.Game
{
    internal class GameStream : AsyncEventStream<Aggregate.Root, Schema.Game.ID>, IGameStream
    {
        public GameStream(IEventStore eventStore, IDECBus publisher) : base(eventStore, publisher)
        {
        }
    }
}