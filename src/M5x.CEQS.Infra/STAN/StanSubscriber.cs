using System;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Core;
using M5x.CEQS.Schema;
using M5x.CEQS.Schema.Extensions;
using Microsoft.Extensions.Hosting;
using NATS.Client;
using Serilog;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace M5x.CEQS.Infra.STAN
{
    public abstract class StanSubscriber<TAggregateId, TFact> : BackgroundService
        where TFact : IFact<TAggregateId> 
        where TAggregateId : IIdentity
    {
        private readonly IEncodedConnection _conn;
        private readonly IFactHandler<TAggregateId,TFact> _handler;
        private readonly ILogger _logger;
        private string _logMessage;
        private IAsyncSubscription _subscription;

        protected StanSubscriber(IEncodedConnection conn,
            IFactHandler<TAggregateId, TFact> handler,
            ILogger logger)
        {
            _conn = conn;
            _handler = handler;
            _logger = logger;
            _conn.OnDeserialize = data =>
            {
                try
                {
                    return JsonSerializer.DeserializeAsync<TFact>(data.AsStream()).Result;
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
                            var evt = (TFact)args.ReceivedObject;
                            request = $"{EventTopic} - [{evt.AggregateId.Value}]";
                            _logger?.Debug($"[{EventTopic}]-HND [{_handler.GetType()}]");
                            await _handler.HandleAsync(evt);
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
            var atts = (TopicAttribute[]) typeof(TFact).GetCustomAttributes(typeof(TopicAttribute), true);
            if (atts.Length == 0) throw new Exception($"Attribute 'Topic' is not defined on {typeof(TFact)}!");
            return atts[0].Id;
        }
        
        
        
    }
}