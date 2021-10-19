using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using M5x.DEC.Commands;
using M5x.DEC.ExecutionResults;
using M5x.DEC.PubSub;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using Microsoft.Extensions.Hosting;
using NATS.Client;
using Serilog;

namespace M5x.DEC.Infra.STAN
{
    public abstract class STANAsyncResponder<TAggregate, TID, THope, TCmd, TFeedback> : BackgroundService,
        IResponder<TAggregate, TID, THope, TCmd, TFeedback>
        where TAggregate : IAggregateRoot<TID>
        where TID : IIdentity
        where THope : IHope
        where TCmd : ICommand<TAggregate, TID, IExecutionResult>
        where TFeedback : IFeedback
    {
        private readonly IAsyncActor<TAggregate, TID, TCmd, THope, TFeedback> _actor;
        private readonly IDECBus _bus;
        private readonly IEncodedConnection _conn;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private string _logMessage;
        private IAsyncSubscription _subscription;

        protected STANAsyncResponder(IEncodedConnection conn,
            IAsyncActor<TAggregate, TID, TCmd, THope, TFeedback> actor,
            ILogger logger)
        {
            _conn = conn;
//            _bus = bus;
            _actor = actor;
            _logger = logger;
            _conn.OnSerialize = o =>
            {
                var res = JsonSerializer.SerializeToUtf8Bytes((TFeedback)o);
                return res;
            };
            _conn.OnDeserialize = data =>
            {
                try
                {
                    return JsonSerializer.DeserializeAsync<THope>(data.AsStream()).Result;
                }
                catch (Exception e)
                {
                    _logger.Error(e.AsApiError().ToString());
                    throw;
                }
            };
        }

        private string HopeTopic => GetHopeTopic();

        private static string GetHopeTopic()
        {
            var atts = (TopicAttribute[])typeof(THope).GetCustomAttributes(typeof(TopicAttribute), true);
            if (atts.Length == 0) throw new Exception($"Attribute 'Topic' is not defined on {typeof(THope)}!");
            return atts[0].Id;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await StartRespondingAsync(cancellationToken);
        }


        private async Task StartRespondingAsync(CancellationToken cancellationToken)
        {
            try
            {
                //              var request = string.Empty;
                if (_conn.State != ConnState.CONNECTED) await WaitForConnection();
                _logMessage = $"[{HopeTopic}]-RSP on [{JsonSerializer.Serialize(_conn.DiscoveredServers)}]";
                _logger?.Debug(_logMessage);
                _logMessage = "";
                _subscription = _conn.SubscribeAsync(HopeTopic, async (sender, args) =>
                {
                    if (args.ReceivedObject is not THope hope) return;
                    _logger?.Debug($"[{HopeTopic}]-HOPE {hope.AggregateId}");
                    var rsp = await _actor.HandleAsync(hope);
                    _conn.Publish(args.Message.Reply, rsp);
                    _conn.Flush();
                    _logger?.Debug($"[{HopeTopic}]-FEEDBACK {rsp.ErrorState.IsSuccessful} ");
                });
            }
            catch (Exception e)
            {
                _logMessage = $"[{HopeTopic}]-ERR {JsonSerializer.Serialize(e.AsApiError())}";
                _logger.Fatal(_logMessage);
                await _subscription.DrainAsync();
            }
        }

        private async Task StopRespondingAsync(CancellationToken cancellationToken)
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


        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await StopRespondingAsync(cancellationToken);
        }

        private Task WaitForConnection()
        {
            return Task.Run(() =>
            {
                while (_conn.State != ConnState.CONNECTED)
                    _logger.Information($"Waiting for Connection. State: {_conn.State}");
            });
        }
    }
}