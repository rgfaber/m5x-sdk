using System;
using System.Threading;
using System.Threading.Tasks;
using EventFlow;
using EventFlow.Aggregates;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;
using EventFlow.Core;
using M5x.CEQS.Schema;
using M5x.CEQS.Schema.Extensions;
using Microsoft.Extensions.Hosting;
using NATS.Client;
using Serilog;
using static System.Threading.Tasks.Task;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace M5x.CEQS.Infra.STAN
{
    public abstract class StanResponder<TAggregate, TID, THope, TCmd, TFeedback> : BackgroundService
        where THope : IHope<TID>
        where TFeedback : IFeedback<TID>
        where TCmd : ICommand<TAggregate, TID, IExecutionResult>
        where TAggregate : IAggregateRoot<TID>
        where TID : IIdentity


    {
        private readonly IEncodedConnection _conn;
        private readonly ICommandBus _bus;
        private readonly ILogger _logger;
        private string _logMessage;
        private IAsyncSubscription _subscription;

        protected StanResponder(IEncodedConnection conn,
            ICommandBus bus,
            ILogger logger)
        {
            _conn = conn;
            _bus = bus;
            _logger = logger;
            _conn.OnSerialize = o =>
            {
                var res = JsonSerializer.SerializeToUtf8Bytes((TFeedback) o);
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
        private string CommandTopic => GetTopic();
        private static string GetTopic()
        {
            var atts = (TopicAttribute[]) typeof(THope).GetCustomAttributes(typeof(TopicAttribute), true);
            if (atts.Length == 0) throw new Exception($"Attribute 'Topic' is not defined on {typeof(THope)}!");
            return atts[0].Id;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Run(() =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                }
            }, stoppingToken);
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await StartRespondingAsync(cancellationToken);
        }


        private async Task StartRespondingAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_conn.State != ConnState.CONNECTED)
                {
                    await WaitForConnection();
                }
                _logMessage = $"[{CommandTopic}]-RSP on [{JsonSerializer.Serialize(_conn.DiscoveredServers)}]";
                _logger?.Debug(_logMessage);
                _logMessage = "";
                _subscription = _conn.SubscribeAsync(CommandTopic, async (sender, args) =>
                {
                    try
                    {
                        if (args.ReceivedObject is not THope hope) return;
                        _logger?.Debug($"[{CommandTopic}]-REQ {JsonSerializer.Serialize(hope)}");
                        var cmd = ToCommand(hope);
                        var result = _bus.PublishAsync(cmd, cancellationToken)
                            .ConfigureAwait(false)
                            .GetAwaiter().GetResult();
                        var rsp = ToResponse(hope, result);
                        _conn.Publish(args.Message.Reply, rsp);
                        _conn.Flush();
                        _logger?.Debug($"[{CommandTopic}]-RSP {JsonSerializer.Serialize(rsp)} ");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                });
            }
            catch (Exception e)
            {
                _logMessage = $"[{CommandTopic}]-ERR {JsonSerializer.Serialize(e.AsApiError())}";
                _logger.Fatal(_logMessage);
            }
        }

        protected abstract TFeedback ToResponse(THope hope, IExecutionResult result);
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


        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await StopRespondingAsync(cancellationToken);
        }        
        
        private Task WaitForConnection()
        {
            return Run( () =>
            {
                while (_conn.State != ConnState.CONNECTED)
                {
                    _logger.Information($"Waiting for Connection. State: {_conn.State}");
                }
            });
        }

        
        
    }
   
}