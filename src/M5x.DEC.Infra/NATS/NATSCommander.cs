using System;
using System.Threading.Tasks;
using M5x.DEC.Commands;
using M5x.DEC.Extensions;
using M5x.Nats;
using M5x.Schemas;
using M5x.Schemas.Extensions;
using MyNatsClient;
using Serilog;

namespace M5x.DEC.Infra.NATS
{
    public abstract class NATSCommander<TCommand, TRsp> : ICommander<TCommand, TRsp>
        where TCommand : Command 
        where TRsp : Response, new()
    {
        private readonly INatsClient _bus;
        private readonly ILogger _logger;

        protected NATSCommander(INatsClient bus, ILogger logger)
        {
            _bus = bus;
            _logger = logger;
        }

        public string Topic => GetTopic();

        public async Task<TRsp> CommandAsync(TCommand cmd) 
            
        {
            var rsp = new TRsp {CorrelationId = cmd.CorrelationId};
            try
            {
                if (!_bus.IsConnected)
                {
                    _logger?.Debug($"::CONNECT bus: {_bus.Id}");
                    await _bus.ConnectAsync();
                }

                _logger?.Debug($"::COMMAND command: {cmd.CorrelationId} on topic {Topic}.");
                var req = await _bus.RequestAsync(Topic, cmd.NatsEncode());
                _logger?.Debug($"::RECEIVED response: {req.SubscriptionId} on {req.ReplyTo}.");
                rsp = req?.Payload.NatsDecode<TRsp>();
            }
            catch (Exception e)
            {
                rsp.State.Errors.Add($"NATSCommander<{typeof(TRsp).PrettyPrint()}>.Exception", e.AsApiError());
                _logger?.Fatal($"::EXCEPTION {e.Message}");
            }
            return rsp;
        }

        protected virtual string GetTopic()
        {
            var attrs = (TopicAttribute[]) typeof(TCommand).GetCustomAttributes(typeof(TopicAttribute), true);
            return attrs.Length > 0 ? attrs[0].Id : $"No Topic Defined on {typeof(TCommand)}!";
        }
    }
}