using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;

namespace M5x.CEQS
{
    public interface IExecute<in TCmd> where TCmd: ICommand
    {
        IExecutionResult Execute(TCmd cmd);
    }
}