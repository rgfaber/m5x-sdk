using System.Text.Json;
using System.Threading.Tasks;
using M5x.CEQS.PubSub;
using M5x.CEQS.Schema;
using Serilog;

namespace M5x.CEQS.Infra.Logger
{
    
    public interface ILogWriter<TAggregateId, TEvent> : IEventHandler<TAggregateId, TEvent> 
        where TAggregateId: IAggregateID
        where TEvent : IEvent<TAggregateId>
    {
        
    }
    
    
    public abstract class LogWriter<TAggregateId, TEvent> : ILogWriter<TAggregateId, TEvent>
        where TEvent : IEvent<TAggregateId>
        where TAggregateId : IAggregateID
    {
        protected ILogger Logger { get; }
        
        protected LogWriter(ILogger logger)
        {
            Logger = logger;
        }

        public virtual async Task HandleAsync(TEvent @event)
        {
            Logger.Information(JsonSerializer.Serialize(@event));
        }
    }
}