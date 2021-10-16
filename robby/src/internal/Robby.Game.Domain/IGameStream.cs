using M5x.DEC.Persistence;

namespace Robby.Game.Domain
{
    public interface IGameStream : IAsyncEventStream<Aggregate.Root, Schema.Game.ID>
    {
    }
}