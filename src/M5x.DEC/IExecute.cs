using System.Threading.Tasks;
using M5x.DEC.Commands;
using M5x.DEC.Schema;

namespace M5x.DEC
{
    public interface IExecute
    {
    }


    public interface IExecute<in TCommand, in TStrategy> : IExecute
        where TCommand : ICommand
        where TStrategy : IStrategy
    {
        Task<IExecutionResult> Execute(TCommand command, TStrategy strategy);
    }

    public interface IExecute<in TCommand> : IExecute
        where TCommand : ICommand
    {
        IExecutionResult Execute(TCommand command);
    }

    public interface IStrategy
    {
    }
}