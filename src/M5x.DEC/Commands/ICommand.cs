using M5x.DEC.ExecutionResults;
using M5x.DEC.Schema;

namespace M5x.DEC.Commands
{
    public interface ICommand<TAggregate, TIdentity, TExecutionResult> : ICommand<TIdentity>
        where TAggregate : IAggregate<TIdentity>
        where TIdentity : IIdentity
        where TExecutionResult : IExecutionResult
    {
    }


    public interface ICommand : IVersionedType, ICorrelated
    {
        ISourceID GetSourceId();
    }

    public interface ICommand<out TIdentity, out TSourceIdentity> : ICommand
        where TIdentity : IIdentity
        where TSourceIdentity : ISourceID
    {
        TIdentity AggregateId { get; }
        TSourceIdentity SourceId { get; }
    }

    public interface ICommand<out TIdentity> : ICommand<TIdentity, ISourceID>
        where TIdentity : IIdentity
    {
    }
}