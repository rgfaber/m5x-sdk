namespace M5x.DEC.Infra;

public interface IInterpreter<TModel, in TEvent>
{
    TModel Interpret(TEvent evt, TModel model);
}