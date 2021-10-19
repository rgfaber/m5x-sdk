using M5x.DEC.Commands;
using M5x.DEC.ExecutionResults;
using M5x.DEC.Schema;

namespace M5x.DEC.Infra
{
    public interface IResponder<TAggregate, TID, THope, TCmd, TFeedback>
        where TAggregate : IAggregateRoot<TID>
        where TID : IIdentity
        where THope : IHope
        where TCmd : ICommand<TAggregate, TID, IExecutionResult>
        where TFeedback : IFeedback {}
}