using Ardalis.GuardClauses;
using M5x.DEC.Infra;
using M5x.DEC.TestKit.Tests.SuT;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.DEC.TestKit.Tests
{
    public static class Inject
    {
        public static IServiceCollection AddMyInterpreter(this IServiceCollection services)
        {
            return services?
                .AddTransient<IInterpreter<MyReadModel, MyEvent>, MyInterpreter>();
        }
    }


    public interface IMyInterpreter : IInterpreter<MyReadModel, MyEvent>
    {
    }

    internal class MyInterpreter : IMyInterpreter
    {
        public MyReadModel Interpret(MyEvent evt, MyReadModel model)
        {
            Guard.Against.BadEvent(evt);
//            Guard.Against.BadMyReadModel(model);
            model ??= MyReadModel.New(TestConstants.Id, TestConstants.Id);
            model.Meta = evt.Meta;
            model.Id = evt.Meta.Id;
            model.Prev = evt.EventId.ToString();
            model.Status = (MyStatus)evt.Meta.Status;
            model.Content = evt.Payload;
            return model;
        }
    }
}