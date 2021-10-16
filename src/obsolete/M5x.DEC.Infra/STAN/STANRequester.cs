using System;
using System.Text.Json;
using System.Threading.Tasks;
using M5x.Schemas;
using M5x.Schemas.Extensions;
using NATS.Client;
using Serilog;

namespace M5x.DEC.Infra.STAN
{
    public abstract class STANRequester<TID, TRequest, TRsp> : IRequester<TRequest, TRsp>
        where TRequest : Request<TID> 
        where TRsp : Response, new()
        where TID : IAggregateID
    {
        private readonly IEncodedConnection _conn;
        private readonly ILogger _logger;
        private string _logMessage;

        public string Topic => GetTopic();
        
        public STANRequester(IEncodedConnection conn, ILogger logger)
        {
            _conn = conn;
            _logger = logger;
            conn.OnSerialize = o => JsonSerializer.SerializeToUtf8Bytes((TRequest)o);
            conn.OnDeserialize = data => JsonSerializer.Deserialize<TRsp>(data);
        }
        
        
        protected virtual string GetTopic()
        {
            var attrs = (TopicAttribute[]) typeof(TRequest).GetCustomAttributes(typeof(TopicAttribute), true);
            return attrs.Length > 0 ? attrs[0].Id : $"No Topic Defined on {typeof(TRequest)}!";
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


        public async Task<TRsp> RequestAsync(TRequest req)
        {
            var rsp = new TRsp {CorrelationId = req.CorrelationId};
            try
            {
                await WaitForConnection();
                if (_conn.State == ConnState.CONNECTED)
                {
                    _logger?.Debug($"[{Topic}]-REQ {JsonSerializer.Serialize(req)}");
                    rsp = (TRsp)_conn.Request(Topic, req);
                    _conn.Flush();
                    _logger?.Debug($"[{Topic}]-RSP {JsonSerializer.Serialize(rsp)}");
                };
            }
            catch (Exception e)
            {
                rsp.ErrorState.Errors.Add($"STANRequester<{typeof(TRsp).PrettyPrint()}>.Exception", e.AsApiError());
                _logger?.Fatal($"[{Topic}]-ERR {JsonSerializer.Serialize(e.AsApiError())}");
            }
            return rsp;
        }
    }
}