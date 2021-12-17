using System.Threading.Tasks;
using M5x.DEC.Events;
using M5x.DEC.Infra;
using M5x.DEC.Schema;
using M5x.Testing;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration.Etl;

public abstract class InterpreterTests<TInterpreter, TModel, TEvent> : IoCTestsBase
    where TInterpreter : IInterpreter<TModel, TEvent>
    where TEvent : IEvent<IIdentity>
{
    protected object Input;

    protected object Interpreter;

    protected object StartModel;

    protected InterpreterTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
    {
    }

    [Fact]
    public Task Needs_Interpreter()
    {
        Assert.NotNull(Interpreter);
        return Task.CompletedTask;
    }

    [Fact]
    public Task Must_InterpreterMustBeAssignableToTInterpreter()
    {
        Interpreter.ShouldBeAssignableTo<TInterpreter>();
        return Task.CompletedTask;
    }

    [Fact]
    public Task Needs_StartModel()
    {
        Assert.NotNull(StartModel);
        return Task.CompletedTask;
    }

    [Fact]
    public Task Must_StartModelMustBeAssignableToTModel()
    {
        StartModel.ShouldBeAssignableTo<TModel>();
        return Task.CompletedTask;
    }

    [Fact]
    public Task Needs_Input()
    {
        Assert.NotNull(Input);
        return Task.CompletedTask;
    }


    [Fact]
    public Task Must_InputMustBeAssignableToTEvent()
    {
        Input.ShouldBeAssignableTo<TEvent>();
        return Task.CompletedTask;
    }

    [Fact]
    public Task Must_InterpreterMustInterpretEventAgainstModel()
    {
        var expected = ((TInterpreter)Interpreter).Interpret(
            (TEvent)Input,
            (TModel)StartModel);
        Assert.NotNull(expected);
        return Task.CompletedTask;
    }
}