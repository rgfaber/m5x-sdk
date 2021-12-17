using System.Threading.Tasks;

namespace M5x.DEC.Sagas.SagaTimeouts;

public interface ISagaHandlesTimeout<TTimeout> : ISaga
    where TTimeout : class, ISagaTimeoutJob
{
    bool HandleTimeout(TTimeout timeout);
}

public interface ISagaHandlesTimeoutAsync<TTimeout> : ISaga
    where TTimeout : class, ISagaTimeoutJob
{
    Task HandleTimeoutAsync(TTimeout timeout);
}