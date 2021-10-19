using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using NATS.Client;
using Polly;
using Polly.Retry;
using Serilog;

namespace M5x.DEC.Infra.STAN
{
    public abstract class STANEmitter<TAggregateId, TFact>
        : IFactEmitter<TAggregateId, TFact>
        where TFact : IFact
        where TAggregateId : IIdentity
    {
        private readonly IEncodedConnection _conn;
        private readonly ILogger _logger;
        private readonly int _maxRetries = Polly.Config.MaxRetries;
        private readonly AsyncRetryPolicy _retryPolicy;
        private int _backoff = 100;
        private string _logMessage;

        protected STANEmitter(IEncodedConnection conn,
            ILogger logger, AsyncRetryPolicy retryPolicy = null
        )
        {
            _conn = conn;
            _logger = logger;
            conn.OnSerialize = o => JsonSerializer.SerializeToUtf8Bytes((TFact)o);
            _retryPolicy = retryPolicy
                           ?? Policy
                               .Handle<Exception>()
                               .WaitAndRetryAsync(_maxRetries,
                                   times => TimeSpan.FromMilliseconds(times * _backoff));
        }

        private string FactTopic => GetFactTopic();

        public async Task EmitAsync(TFact fact, CancellationToken cancellationToken=default)
        {
                try
                {
                    if (_conn.State != ConnState.CONNECTED)
                        await WaitForConnection()
                            .ConfigureAwait(false);
                    if (_logger != null)
                    {
                        _logMessage = $"[{FactTopic}]-EMIT  {fact.Meta}";
                        _logger?.Debug(_logMessage);
                    }
                    _conn?.Publish(FactTopic, fact);
                    _conn?.Flush();
                }
                catch (Exception e)
                {
                    _logger?.Fatal(
                        $"[{FactTopic}]-ERR {JsonSerializer.Serialize(e.AsApiError())}");
                    throw;
                }
            
        }


        private static string GetFactTopic()
        {
            var atts = (TopicAttribute[])typeof(TFact).GetCustomAttributes(typeof(TopicAttribute), true);
            if (atts.Length == 0) throw new Exception($"Attribute 'Topic' is not defined on {typeof(TFact)}!");
            return atts[0].Id;
        }

        private Task WaitForConnection()
        {
            return Task.Run(() =>
            {
                while (_conn.State != ConnState.CONNECTED)
                {
                    _logger.Information($"Waiting for Connection. State: {_conn.State}");
                    Thread.Sleep(10000);
                }
            });
        }
    }
}