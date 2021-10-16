using M5x.DEC.Commands;
using M5x.DEC.ExecutionResults;

namespace M5x.DEC
{
    public interface IExecute
    {
    }

    public interface IExecute<in TCommand> : IExecute
        where TCommand : ICommand
    {
        IExecutionResult Execute(TCommand command);
    }
}