using System;
using System.Threading;
using System.Threading.Tasks;
using M5x.DEC.Schema;

namespace M5x.DEC
{

    public interface IRequester: IDisposable
    {
        string Topic { get; }
    }
    
    public interface IRequester<in THope, TExecutionResult>: IRequester
        where THope : IHope
        where TExecutionResult : IExecutionResult
    {
        Task<TExecutionResult> RequestAsync(THope hope, CancellationToken cancellationToken = default);
    }
}