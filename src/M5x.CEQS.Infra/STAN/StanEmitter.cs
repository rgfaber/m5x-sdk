using System;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates;
using EventFlow.Core;
using M5x.CEQS.Schema;
using M5x.CEQS.Schema.Extensions;
using NATS.Client;
using Serilog;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace M5x.CEQS.Infra.STAN
{
    public abstract class StanEmitter<TAggregate,TAggregateId, TEvent>: IEventEmitter<TAggregate,TAggregateId, TEvent> 
        where TAggregate : IAggregateRoot<TAggregateId>
        where TAggregateId : IIdentity
        where TEvent : IAggregateEvent<TAggregate, TAggregateId>
    {
        private readonly IEncodedConnection _conn;
        private readonly ILogger _logger;
        private string _logMessage;

        protected StanEmitter(IEncodedConnection conn, ILogger logger)
        {
            _conn = conn;
            _logger = logger;
            conn.OnSerialize = o => JsonSerializer.SerializeToUtf8Bytes((TEvent)o);
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


        public async Task HandleAsync(IDomainEvent<TAggregate, TAggregateId, TEvent> domainEvent, CancellationToken cancellationToken)
        {
            try
            {
                if (_conn.State != ConnState.CONNECTED)
                {
                    await WaitForConnection();
                }
                if (_logger != null)
                {
                    _logMessage = $"[{Topic}]-PUB  {JsonSerializer.Serialize(domainEvent)}";
                    _logger?.Debug(_logMessage);
                }
                _conn?.Publish(Topic, domainEvent);
                _conn?.Flush();
            }
            catch (Exception e)
            {
                _logger?.Fatal(
                    $"[{Topic}]-ERR {JsonSerializer.Serialize(e.AsApiError())}");
            }
        }
    }
}