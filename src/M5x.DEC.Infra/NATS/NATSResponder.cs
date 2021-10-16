using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using M5x.DEC.Commands;
using M5x.DEC.Exceptions;
using M5x.Nats;
using M5x.Schemas;
using Microsoft.Extensions.Hosting;
using MyNatsClient;
using MyNatsClient.Rx;
using Serilog;

namespace M5x.DEC.Infra.NATS
{
    public abstract class NATSResponder<TAggregateId,
        TCmd,
        TRequest,
        TRsp> : BackgroundService
        where TAggregateId : IAggregateID
        where TCmd : ICommand
        where TRequest : IRequest
        where TRsp : Response
    {
        private readonly IActor<TAggregateId, TRequest,TRsp, TCmd> _actor;
        private readonly INatsClient _bus;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private CancellationTokenSource _cts;
        private string _logMessage;
        private ISubscription _subscription;

        protected NATSResponder(INatsClient bus, 
            IActor<TAggregateId, TRequest, TRsp, TCmd> actor, 
            ILogger logger,
            IMapper mapper)
        {
            _bus = bus;
            _actor = actor;
            _logger = logger;
            _mapper = mapper;
        }

        private string CommandTopic => GetTopic();

        private static string GetTopic()
        {
            var atts = (TopicAttribute[]) typeof(TRequest).GetCustomAttributes(typeof(TopicAttribute), true);
            if (atts.Length == 0) throw new Exception($"Attribute 'Topic' is not defined on {typeof(TRequest)}!");
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
                var request = string.Empty;
                if (!_bus.IsConnected)
                {
                    _logMessage = $"::CONNECT ::Bus [{_bus?.Id}]";
                    _logger?.Debug(_logMessage);
                    await _bus.ConnectAsync();
                }

                _logMessage = $"::RESPOND ::Topic: [{CommandTopic}] on bus [{_bus?.Id}]";
                _logger?.Debug(_logMessage);
                _logMessage = "";
                _subscription = await _bus.SubAsync(CommandTopic,
                    stream => stream.SubscribeSafe(
                        async msg =>
                        {
                            var req = msg.Payload.NatsDecode<TRequest>();
                            request = $"{CommandTopic} - [{req.CorrelationId}]";
                            _logger?.Debug($"::RECEIVED: {request} ::Payload: {req.CorrelationId}");
                            var rsp = await _actor.Handle(req);
                            await _bus.PubAsync(msg.ReplyTo, rsp.NatsEncode());
                            _logger?.Debug(
                                $"::RESPONDED: {request}  ::Payload: {rsp.CorrelationId}-{rsp.State.IsSuccessful} ");
                        },
                        exception => { _logger.Error($"::ERROR: ${request}  {exception.Message} "); },
                        () => { _logger.Debug($"::COMPLETED: {request}"); }));
            }
            catch (Exception e)
            {
                _logMessage = $"::EXCEPTION {e.Message}";
                throw;
            }
        }

        private async Task StopResondingAsync(CancellationToken cancellationToken)
        {
            if (_bus.IsConnected)
            {
                _logMessage = $"::UNSUBSCRIBING ::Topic: [{CommandTopic}]";
                _logger?.Debug(_logMessage);
                await _bus.UnsubAsync(_subscription);
                _logMessage = $"::DISCONNECTING ::Bus: [{_bus.Id}]";
                _logger?.Debug(_logMessage);
                _bus.Disconnect();
            };
        }


        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await StopResondingAsync(cancellationToken);
        }
    }
}