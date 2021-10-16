using System;
using System.Text.Json;
using System.Threading.Tasks;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using NATS.Client;
using Polly;
using Polly.Retry;
using Serilog;

namespace M5x.DEC.Infra.STAN
{
    public abstract class STANRequester<THope, TFeedback> : IDisposable, IRequester<THope, TFeedback>
        where THope : IHope
        where TFeedback : IFeedback
    {
        private readonly IEncodedConnection _conn;
        private readonly ILogger _logger;
        private readonly int _maxRetries = Polly.Config.MaxRetries;
        private readonly AsyncRetryPolicy _retryPolicy;
        private string _logMessage;

        public STANRequester(IEncodedConnection conn, ILogger logger, AsyncRetryPolicy retryPolicy = null)
        {
            _conn = conn;
            _logger = logger;
            _retryPolicy = retryPolicy
                           ?? Policy
                               .Handle<Exception>()
                               .WaitAndRetryAsync(_maxRetries,
                                   times => TimeSpan.FromMilliseconds(times * 100));
            conn.OnSerialize = o => JsonSerializer.SerializeToUtf8Bytes((THope)o);
            conn.OnDeserialize = data => JsonSerializer.Deserialize<TFeedback>(data);
        }

        public string HopeTopic => GetHopeTopic();

        public void Dispose()
        {
            _conn?.Dispose();
        }


        public async Task<TFeedback> RequestAsync(THope req)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                TFeedback rsp = default;
                try
                {
                    await WaitForConnection();
                    if (_conn.State == ConnState.CONNECTED)
                    {
                        _logger?.Debug($"[{HopeTopic}]-HOPE {JsonSerializer.Serialize(req)}");
                        rsp = (TFeedback)_conn.Request(HopeTopic, req);
                        _conn.Flush();
                        _logger?.Debug($"[{HopeTopic}]-FEEDBACK {JsonSerializer.Serialize(rsp)}");
                    }

                    ;
                }
                catch (Exception e)
                {
                    rsp.ErrorState.Errors.Add($"STANRequester<{typeof(TFeedback).PrettyPrint()}>.Exception",
                        e.AsApiError());
                    _logger?.Fatal($"[{HopeTopic}]-ERR {JsonSerializer.Serialize(e.AsApiError())}");
                    throw;
                }

                return rsp;
            });
        }


        protected virtual string GetHopeTopic()
        {
            var attrs = (TopicAttribute[])typeof(THope).GetCustomAttributes(typeof(TopicAttribute), true);
            return attrs.Length > 0 ? attrs[0].Id : $"No Topic Defined on {typeof(THope)}!";
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