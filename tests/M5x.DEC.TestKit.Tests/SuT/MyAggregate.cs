using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;
using M5x.DEC.ExecutionResults;
using M5x.DEC.Schema;

namespace M5x.DEC.TestKit.Tests.SuT;

public class MyAggregate : AggregateRoot<MyID, MyReadModel, MyStatus>,
    IExecute<MyCommand>,
    IApply<MyEvent>
{
    public MyAggregate()
    {
    }

    public MyAggregate(MyID id, MyReadModel model) : base(id, model)
    {
    }

    public void Apply(MyEvent evt)
    {
    }

    public IExecutionResult Execute(MyCommand command)
    {
        var errors = new List<string>();
        try
        {
            Guard.Against.Null(command, nameof(command));
            Guard.Against.Null(command.Payload, nameof(command.Payload));
            RaiseEvent(MyEvent.New(command));
            return ExecutionResult.Success();
        }
        catch (AggregateException ae)
        {
            ae.Handle(x =>
            {
                errors.Add(ae.Message);
                return true;
            });
            return ExecutionResult.Failed(errors);
        }
    }

    protected override MyReadModel CreateModel()
    {
        return MyTestSchema.Model;
    }

    public static MyAggregate New(MyID myId, MyReadModel model)
    {
        return new MyAggregate(myId, model);
    }
}