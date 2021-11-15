using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using M5x.DEC.Persistence;
using M5x.DEC.PubSub;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using Microsoft.Extensions.Hosting;
using NATS.Client;
using Serilog;

namespace M5x.DEC.Infra.STAN
{
    public abstract class STANSubscriber<TAggregateId, TFact> : BackgroundService
        where TAggregateId : IIdentity
        where TFact : IFact
    {
        private readonly IDECBus _bus;
        private readonly IEnumerable<IFactHandler<TAggregateId, TFact>> _handlers;
        private readonly ILogger _logger;
        protected readonly IEncodedConnection Conn;
        private string _logMessage;
        private IAsyncSubscription _subscription;

        protected STANSubscriber(IEncodedConnection conn,
            IDECBus bus,
            IEnumerable<IFactHandler<TAggregateId, TFact>> handlers,
            ILogger logger)
        {
            Conn = conn;
            _bus = bus;
            _handlers = handlers;
            _logger = logger;
            Conn.OnDeserialize = data =>
            {
                try
                {
                    return JsonSerializer.Deserialize<TFact>(data);
                }
                catch (Exception e)
                {
                    _logger.Fatal(e.InnerAndOuter());
                    throw;
                }
            };
            Conn.Opts.ClosedEventHandler = (sender, args) => { Conn.ResetStats(); };
        }


        public string Topic => GetTopic();


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
            }

            return Task.CompletedTask;
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
            if (Conn.State == ConnState.CONNECTED)
            {
                _logMessage = $"[CONN]-DRN [{Conn.ConnectedUrl}]";
                _logger?.Debug(_logMessage);
                await Conn.DrainAsync();
                _logMessage = $"[CONN]-CLS [{Conn.ConnectedUrl}]";
                _logger?.Debug(_logMessage);
                Conn.Close();
            }
        }


        private async Task StartProcessing(CancellationToken cancellationToken)
        {
            try
            {
                if (Conn.State != ConnState.CONNECTED) await WaitForConnection();
                _logMessage = $"[{Topic}]-SUB on [{JsonSerializer.Serialize(Conn.DiscoveredServers)}]";
                _logger?.Debug(_logMessage);
                _logMessage = "";
                if (Conn.State == ConnState.CONNECTED)
                    _subscription = Conn.SubscribeAsync(Topic, (sender, args) =>
                    {
                        var fact = (TFact)args.ReceivedObject;
                        //  await AcknowledgeAsync($"{FactTopic}.Ack", fact);
                        _bus.Subscribe<TFact>(HandleFactAsync);
                        _bus.PublishAsync(fact);
                    });
            }
            catch (Exception e)
            {
                _logMessage = $"[{Topic}]-ERR {JsonSerializer.Serialize(e.AsApiError())})";
                if (_logger != null) _logger.Fatal(_logMessage);
                await _subscription.DrainAsync();
            }
        }

        private Task HandleFactAsync(TFact fact)
        {
            foreach (var handler in _handlers)
            {
                _logger?.Debug($"[{Topic}]-FACT {fact.Meta.Id} \n\t- HND [{handler.GetType()}]");
                handler.HandleAsync(fact);
            }

            return Task.CompletedTask;
        }


        private Task AcknowledgeAsync(string topic, TFact fact)
        {
            var ack = DefaultAck.FromFact(GetType().PrettyPrint(), fact);
            Conn.Publish(topic, ack);
            return Task.CompletedTask;
        }


        private Task WaitForConnection()
        {
            var res = Task.Run(() =>
            {
                while (Conn.State != ConnState.CONNECTED)
                    _logger.Information($"Waiting for Connection. State: {Conn.State}");
            });
            return res;
        }

        private string GetTopic()
        {
            var atts = (TopicAttribute[])typeof(TFact).GetCustomAttributes(typeof(TopicAttribute), true);
            if (atts.Length == 0) throw new Exception($"Attribute 'Topic' is not defined on {typeof(TFact)}!");
            return atts[0].Id;
        }
    }
}