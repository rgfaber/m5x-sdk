using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using M5x.DEC.Events;
using M5x.DEC.Schema;
using Serilog;

namespace M5x.DEC.Infra.Logger
{
    public interface ILogWriter<TAggregateId, TEvent> : IEventHandler<TAggregateId, TEvent>
        where TAggregateId : IIdentity
        where TEvent : IEvent<TAggregateId>
    {
    }


    public abstract class LogWriter<TAggregateId, TEvent> : ILogWriter<TAggregateId, TEvent>
        where TEvent : IEvent<TAggregateId>
        where TAggregateId : IIdentity
    {
        protected LogWriter(ILogger logger)
        {
            Logger = logger;
        }

        protected ILogger Logger { get; }

        public Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default)
        {
            Logger.Information(JsonSerializer.Serialize(@event));
            return Task.CompletedTask;
        }
    }
}