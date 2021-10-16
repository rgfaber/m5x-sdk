using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using M5x.Schemas;
using Microsoft.Extensions.Hosting;
using NATS.Client;
using Serilog;

namespace M5x.DEC.Infra.STAN
{
    public abstract class STANListener<TAggregateId, TEvent> : BackgroundService
        where TAggregateId : IAggregateID
        where TEvent : IEvent<TAggregateId>
    {
        private readonly IEncodedConnection _conn;
        private readonly IEnumerable<IAggregateEventHandler<TAggregateId, TEvent>> _handlers;
        private readonly ILogger _logger;
        private string _logMessage;
        private IAsyncSubscription _subscription;

        protected STANListener(IEncodedConnection conn,
            IEnumerable<IAggregateEventHandler<TAggregateId, TEvent>> handlers,
            ILogger logger)
        {
            _conn = conn;
            _handlers = handlers;
            _logger = logger;
            _conn.OnDeserialize = data =>
            {
                try
                {
                    return JsonSerializer.DeserializeAsync<TEvent>(data.AsStream()).Result;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            };
            _conn.Opts.ClosedEventHandler = (sender, args) =>
            {
                _conn.ResetStats();
            };
        }
        
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
            }
        }
        
        
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await StartProcessing(cancellationToken);
        }


        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await StopProcessing(cancellationToken);
        }
        
        
         private async Task StopProcessing(CancellationToken cancellationToken)
        {
            if (_conn.State == ConnState.CONNECTED)
            {
                _logMessage = $"[CONN]-DRN [{_conn.ConnectedUrl}]";
                _logger?.Debug(_logMessage);
                await _conn.DrainAsync();
                _logMessage = $"[CONN]-CLS [{_conn.ConnectedUrl}]";
                _logger?.Debug(_logMessage);
                _conn.Close();
            }
        }
                

        private async Task StartProcessing(CancellationToken cancellationToken)
        {
            try
            {
                var request = string.Empty;
                if (_conn.State != ConnState.CONNECTED)
                {
                    await WaitForConnection();
                }
                _logMessage = $"[{EventTopic}]-SUB on [{JsonSerializer.Serialize(_conn.DiscoveredServers)}]";
                _logger?.Debug(_logMessage);
                _logMessage = "";
                if (_conn.State == ConnState.CONNECTED)
                {
                    _subscription = _conn.SubscribeAsync(EventTopic, async (sender, args) =>
                        {
                            TEvent evt = (TEvent)args.ReceivedObject;
                            request = $"{EventTopic} - [{evt.EventId}]";
                            _logger?.Debug($"[{EventTopic}]-EVT {JsonSerializer.Serialize(evt)}");
                            foreach (var handler in _handlers)
                            {
                                _logger?.Debug($"[{EventTopic}]-HND [{handler.GetType()}]");
                                await handler.HandleAsync(evt);
                            }
                        });
                }
            }
            catch (Exception e)
            {
                _logger?.Fatal($"[{EventTopic}]-ERR {JsonSerializer.Serialize(e.AsApiError())})");
                throw;
            }
        }
        

        
        
        private Task WaitForConnection()
        {
            var res = Task.Run( (() =>
            {
                while (_conn.State != ConnState.CONNECTED)
                {
                    _logger.Information($"Waiting for Connection. State: {_conn.State}");
                }
            }));
            return res;
        }

 
        private string EventTopic => GetTopic();

        private static string GetTopic()
        {
            var atts = (TopicAttribute[]) typeof(TEvent).GetCustomAttributes(typeof(TopicAttribute), true);
            if (atts.Length == 0) throw new Exception($"Attribute 'Topic' is not defined on {typeof(TEvent)}!");
            return atts[0].Id;
        }
        
        
        
    }
}