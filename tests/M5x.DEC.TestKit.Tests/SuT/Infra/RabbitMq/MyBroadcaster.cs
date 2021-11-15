using System.Threading.Tasks;
using M5x.DEC.Events;
using M5x.DEC.PubSub;
using M5x.DEC.TestKit.Tests.SuT.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.DEC.TestKit.Tests.SuT.Infra.RabbitMq
{
    public static partial class Inject
    {
        public static IServiceCollection AddMyBroadcaster(this IServiceCollection services)
        {
            return services?
                .AddTransient<IMyBroadcaster, MyBroadcaster>();
        }
    }


    internal class MyBroadcaster : Broadcaster<MyID>, IMyBroadcaster
    {
        private readonly IMyEventEmitter _myEventEmitter;

        public MyBroadcaster(IDECBus mediator,
            IMyEventEmitter myEventEmitter) : base(mediator)
        {
            _myEventEmitter = myEventEmitter;
        }

        public override Task BroadcastAsync(IEvent<MyID> evt)
        {
            if (evt is not MyEvent e) return Task.CompletedTask;
            _myEventEmitter.HandleAsync(e);
            return Task.CompletedTask;
        }
    }
}