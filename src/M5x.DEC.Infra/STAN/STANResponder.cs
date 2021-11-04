using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using M5x.DEC.Commands;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using Microsoft.Extensions.Hosting;
using NATS.Client;
using Serilog;

namespace M5x.DEC.Infra.STAN
{
    public abstract class STANResponder<TID, THope, TCmd, TFeedback> : BackgroundService,
        IResponder<TID, THope, TCmd, TFeedback>
        where TID : IIdentity
        where THope : IHope
        where TCmd : ICommand<TID>
        where TFeedback : IFeedback
    {
        private readonly IActor<TID, TCmd, TFeedback> _actor;
        private readonly IEncodedConnection _conn;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private string _logMessage;
        private IAsyncSubscription _subscription;

        

        protected STANResponder(IEncodedConnection conn,
            IActor<TID, TCmd, TFeedback> actor,
            ILogger logger)
        {
            _conn = conn;
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
                    Console.WriteLine(e);
                    throw;
                }
            };
        }

        public string Topic => GetTopic();

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await StartRespondingAsync(cancellationToken);
        }


        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await StopRespondingAsync(cancellationToken);
        }

        private string GetTopic()
        {
            var atts = (TopicAttribute[])typeof(THope).GetCustomAttributes(typeof(TopicAttribute), true);
            if (atts.Length == 0) throw new Exception($"Attribute 'Topic' is not defined on {typeof(THope)}!");
            return atts[0].Id;
        }

        protected override  Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
            }
            return Task.CompletedTask;
        }


        private async Task StartRespondingAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_conn.State != ConnState.CONNECTED) await WaitForConnection();
                _logMessage = $"[{Topic}]-RSP on [{JsonSerializer.Serialize(_conn.DiscoveredServers)}]";
                _logger?.Debug(_logMessage);
                _logMessage = "";
                _subscription = _conn.SubscribeAsync(Topic, async (sender, args) =>
                {
                    if (args.ReceivedObject is not THope hope) return;
                    _logger?.Debug($"[{Topic}]-REQ {JsonSerializer.Serialize(hope)}");
                    var cmd = ToCommand(hope);
                    var rsp = _actor.Handle(cmd);
                    _conn.Publish(args.Message.Reply, rsp);
                    _conn.Flush();
                    _logger?.Debug($"[{Topic}]-RSP {JsonSerializer.Serialize(rsp)} ");
                });
            }
            catch (Exception e)
            {
                _logMessage = $"[{Topic}]-ERR {JsonSerializer.Serialize(e.AsApiError())}";
                _logger.Fatal(_logMessage);
            }
        }

        protected abstract TCmd ToCommand(THope hope);

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