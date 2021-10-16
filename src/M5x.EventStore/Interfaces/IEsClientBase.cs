using System;

namespace M5x.EventStore.Interfaces
{
  public interface IEsClientBase : IDisposable, IAsyncDisposable
  {
    string ConnectionName { get; }
  }
}
