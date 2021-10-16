using M5x.Schemas.Commands;


namespace M5x.DEC
{
    public interface IExecute
    {
    }

    public interface IExecute<in TCommand> : IExecute
        where TCommand : ICommand
    {
        bool Execute(TCommand command);
    }
}