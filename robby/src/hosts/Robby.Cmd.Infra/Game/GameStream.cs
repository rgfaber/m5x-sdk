using M5x.DEC.Persistence;
using M5x.DEC.Persistence.EventStore;
using M5x.DEC.PubSub;
using Robby.Game.Domain;


namespace Robby.Cmd.Infra.Game
{
    internal class GameStream : AsyncEventStream<Robby.Game.Domain.Aggregate.Root, 
        Robby.Game.Schema.GameModel.ID>, IGameStream
    {
        public GameStream(IEventStore eventStore, IDECBus publisher) : base(eventStore, publisher)
        {
        }
    }
}