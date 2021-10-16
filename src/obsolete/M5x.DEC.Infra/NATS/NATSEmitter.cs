using System;
using System.Threading.Tasks;
using M5x.Nats;
using M5x.Schemas;
using MyNatsClient;
using Newtonsoft.Json;
using Serilog;

namespace M5x.DEC.Infra.NATS
{
    public abstract class NATSEmitter<TAggregateId, TEvent> : IEventEmitter<TAggregateId, TEvent>
        where TEvent : Event<TAggregateId>
        where TAggregateId : IAggregateID
    {
        private readonly INatsClient _bus;
        private readonly ILogger _logger;
        private string _logMessage;

        protected NATSEmitter(INatsClient bus, ILogger logger)
        {
            _bus = bus;
            _logger = logger;
        }

        private string Topic => GetTopic();

        public async Task HandleAsync(TEvent @event)
        {
            try
            {
                if (_bus != null)
                {
                    if (!_bus.IsConnected)
                    {
                        _logMessage = $"::CONNECT ::Bus [{_bus?.Id}]";
                        _logger?.Debug(_logMessage);
                        await _bus.ConnectAsync();
                    }
                    _logMessage = $"::EMIT ::Event: [{Topic}] on bus [{_bus?.Id}] ::Payload: {@event.EventId}";
                    _logger?.Debug(_logMessage);
                    await _bus.PubAsync(Topic, @event.NatsEncode());
                }
            }
            catch (Exception e)
            {
                _logger?.Fatal(
                    $"::EXCEPTION :: [{@event.EventId}] ::Exception {JsonConvert.SerializeObject(e)}");
                throw;
            }
        }


        private static string GetTopic()
        {
            var atts = (TopicAttribute[]) typeof(TEvent).GetCustomAttributes(typeof(TopicAttribute), true);
            if (atts.Length == 0) throw new Exception($"Attribute 'Topic' is not defined on {typeof(TEvent)}!");
            return atts[0].Id;
        }
    }
}