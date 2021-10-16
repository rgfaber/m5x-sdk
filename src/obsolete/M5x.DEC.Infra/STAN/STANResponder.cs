using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using M5x.DEC.Infra.EventStore;
using M5x.Schemas;
using M5x.Schemas.Commands;
using Microsoft.Extensions.Hosting;
using NATS.Client;
using Serilog;


namespace M5x.DEC.Infra.STAN
{
    public abstract class STANResponder<TID, TReq, TCmd, TRsp> : BackgroundService
        where TID : IAggregateID
        where TReq : Request<TID>
        where TCmd: Command<TID>
        where TRsp : Response
    {
        private readonly IEncodedConnection _conn;
        private readonly IActor<TID, TCmd, TRsp> _actor;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private string _logMessage;
        private IAsyncSubscription _subscription;

        protected STANResponder(IEncodedConnection conn,
            IActor<TID, TCmd, TRsp> actor,
            ILogger logger)
        {
            _conn = conn;
            _actor = actor;
            _logger = logger;
            _conn.OnSerialize = o =>
            {
                var res = JsonSerializer.SerializeToUtf8Bytes((TRsp) o);
                return res;
            };
            _conn.OnDeserialize = data =>
            {
                try
                {
                    return JsonSerializer.DeserializeAsync<TReq>(data.AsStream()).Result;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            };
        }
        private string CommandTopic => GetTopic();
        private static string GetTopic()
        {
            var atts = (TopicAttribute[]) typeof(TCmd).GetCustomAttributes(typeof(TopicAttribute), true);
            if (atts.Length == 0) throw new Exception($"Attribute 'Topic' is not defined on {typeof(TCmd)}!");
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
                if (_conn.State != ConnState.CONNECTED)
                {
                    await WaitForConnection();
                }
                _logMessage = $"[{CommandTopic}]-RSP on [{JsonSerializer.Serialize(_conn.DiscoveredServers)}]";
                _logger?.Debug(_logMessage);
                _logMessage = "";
                _subscription = _conn.SubscribeAsync(CommandTopic, async (sender, args) =>
                {
                    if (args.ReceivedObject is not TReq req) return;
                    _logger?.Debug($"[{CommandTopic}]-REQ {JsonSerializer.Serialize(req)}");
                    var cmd = ToCommand(req);
                    var rsp = await _actor.Handle(cmd);
                    _conn.Publish(args.Message.Reply, rsp);
                    _conn.Flush();
                    _logger?.Debug($"[{CommandTopic}]-RSP {JsonSerializer.Serialize(rsp)} ");
                });
            }
            catch (Exception e)
            {
                _logMessage = $"[{CommandTopic}]-ERR {JsonSerializer.Serialize(e.AsApiError())}";
                _logger.Fatal(_logMessage);
            }
        }

        protected abstract TCmd ToCommand(TReq req);

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
            return Task.Run( () =>
            {
                while (_conn.State != ConnState.CONNECTED)
                {
                    _logger.Information($"Waiting for Connection. State: {_conn.State}");
                }
            });
        }

        
        
    }
}