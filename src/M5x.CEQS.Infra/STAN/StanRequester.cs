using System;
using System.Reflection;
using System.Threading.Tasks;
using EventFlow.Core;
using EventFlow.Extensions;
using M5x.CEQS.Schema;
using M5x.CEQS.Schema.Extensions;
using NATS.Client;
using Serilog;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace M5x.CEQS.Infra.STAN
{
    public abstract class StanRequester<TID, THope, TFeedback> : IRequester<THope, TFeedback>
        where THope : IHope<TID> 
        where TFeedback : IFeedback, new()
        where TID : IIdentity
    {
        private readonly IEncodedConnection _conn;
        private readonly ILogger _logger;
        private string _logMessage;

        public string Topic => GetTopic();
        
        public StanRequester(IEncodedConnection conn, ILogger logger)
        {
            _conn = conn;
            _logger = logger;
            conn.OnSerialize = o => JsonSerializer.SerializeToUtf8Bytes((THope)o);
            conn.OnDeserialize = data => JsonSerializer.Deserialize<TFeedback>(data);
        }
        
        
        protected virtual string GetTopic()
        {
            var attrs = (TopicAttribute[]) typeof(THope).GetCustomAttributes(typeof(TopicAttribute), true);
            return attrs.Length > 0 ? attrs[0].Id : $"No Topic Defined on {typeof(THope)}!";
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


        private TFeedback CreateFeedback(TID id, string correlationId)
        {
            try
            {
                return (TFeedback)Activator.CreateInstance(typeof(TFeedback), id, correlationId);
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null)
                {
                    throw e.InnerException;
                }
                throw;
            }
        }



        public async Task<TFeedback> RequestAsync(THope req)
        {
            var rsp = CreateFeedback(req.AggregateId, req.CorrelationId); 
            try
            {
                await WaitForConnection();
                if (_conn.State == ConnState.CONNECTED)
                {
                    _logger?.Debug($"[{Topic}]-REQ {JsonSerializer.Serialize(req)}");
                    rsp = (TFeedback)_conn.Request(Topic, req);
                    _conn.Flush();
                    _logger?.Debug($"[{Topic}]-RSP {JsonSerializer.Serialize(rsp)}");
                };
            }
            catch (Exception e)
            {
                rsp.ErrorState.Errors.Add($"STANRequester<{typeof(TFeedback).PrettyPrint()}>.Exception", e.AsApiError());
                _logger?.Fatal($"[{Topic}]-ERR {JsonSerializer.Serialize(e.AsApiError())}");
            }
            return rsp;
        }
    }
}