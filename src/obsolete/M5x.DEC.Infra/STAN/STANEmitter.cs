using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using M5x.Schemas;
using NATS.Client;
using Serilog;

namespace M5x.DEC.Infra.STAN
{
    public abstract class STANEmitter<TAggregateId, TEvent>: IEventEmitter<TAggregateId, TEvent>
    where TEvent : IEvent<TAggregateId>
    where TAggregateId : IAggregateID
    {
        private readonly IEncodedConnection _conn;
        private readonly ILogger _logger;
        private string _logMessage;

        protected STANEmitter(IEncodedConnection conn, ILogger logger)
        {
            _conn = conn;
            _logger = logger;
            conn.OnSerialize = o => JsonSerializer.SerializeToUtf8Bytes((TEvent)o);
        }

        public async Task HandleAsync(TEvent @event)
        {
            try
            {
                if (_conn.State != ConnState.CONNECTED)
                {
                   await WaitForConnection();
                }
                if (_logger != null)
                {
                    _logMessage = $"[{Topic}]-PUB  {JsonSerializer.Serialize(@event)}";
                    _logger?.Debug(_logMessage);
                }
                _conn?.Publish(Topic, @event);
                _conn?.Flush();
            }
            catch (Exception e)
            {
                _logger?.Fatal(
                    $"[{Topic}]-ERR {JsonSerializer.Serialize(e.AsApiError())}");
            }
        }
        
        private string Topic => GetTopic();
        
        private static string GetTopic()
        {
            var atts = (TopicAttribute[]) typeof(TEvent).GetCustomAttributes(typeof(TopicAttribute), true);
            if (atts.Length == 0) throw new Exception($"Attribute 'Topic' is not defined on {typeof(TEvent)}!");
            return atts[0].Id;
        }
        
        private Task WaitForConnection()
        {
            return Task.Run( () =>
            {
                while (_conn.State != ConnState.CONNECTED)
                {
                    _logger.Information($"Waiting for Connection. State: {_conn.State}");
                    Thread.Sleep(10000);
                }
            });
        }


        
    }
}