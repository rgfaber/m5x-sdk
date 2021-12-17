using M5x.DEC.Persistence;

namespace M5x.DEC.TestKit.Tests.SuT;

public interface IMyEventStream : IAsyncEventStream<MyAggregate, MyID>
{
}