using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using M5x.Consul.Transaction;
#if !(CORECLR || PORTABLE || PORTABLE40)
#endif

namespace M5x.Consul.Client
{
    /// <summary>
    ///     The result of a Consul API query
    /// </summary>
    /// <typeparam name="T">Must be able to be deserialized from JSON</typeparam>
    public class QueryResult<T> : QueryResult
    {
        public QueryResult()
        {
        }

        public QueryResult(QueryResult other) : base(other)
        {
        }

        public QueryResult(QueryResult other, T value) : base(other)
        {
            Response = value;
        }

        /// <summary>
        ///     The result of the query
        /// </summary>
        public T Response { get; set; }
    }

    /// <summary>
    ///     The result of a Consul API write
    /// </summary>
    /// <typeparam name="T">
    ///     Must be able to be deserialized from JSON. Some writes return nothing, in which case this should be
    ///     an empty Object
    /// </typeparam>
    public class WriteResult<T> : WriteResult
    {
        public WriteResult()
        {
        }

        public WriteResult(WriteResult other) : base(other)
        {
        }

        public WriteResult(WriteResult other, T value) : base(other)
        {
            Response = value;
        }

        /// <summary>
        ///     The result of the write
        /// </summary>
        public T Response { get; set; }
    }

    public class PutRequest<TIn, TOut> : ConsulRequest
    {
        private readonly TIn _body;

        public PutRequest(ConsulClient.ConsulClient client, string url, TIn body, WriteOptions options = null) : base(
            client, url,
            HttpMethod.Put)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentException(nameof(url));
            _body = body;
            Options = options ?? WriteOptions.Default;
        }

        public WriteOptions Options { get; set; }

        public async Task<WriteResult<TOut>> Execute(CancellationToken ct)
        {
            Client.CheckDisposed();
            Timer.Start();
            var result = new WriteResult<TOut>();

            HttpContent content = null;

            if (typeof(TIn) == typeof(byte[]))
            {
                var bodyBytes = _body as byte[];
                if (bodyBytes != null) content = new ByteArrayContent(bodyBytes);
                // If body is null and should be a byte array, then just don't send anything.
            }
            else
            {
                content = new ByteArrayContent(Serialize(_body));
            }

            var message = new HttpRequestMessage(HttpMethod.Put, BuildConsulUri(Endpoint, Params));
            ApplyHeaders(message, Client.Config);
            message.Content = content;
            var response = await Client.HttpClient.SendAsync(message, ct).ConfigureAwait(false);

            result.StatusCode = response.StatusCode;

            ResponseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode && (
                    response.StatusCode != HttpStatusCode.NotFound && typeof(TOut) != typeof(TxnResponse) ||
                    response.StatusCode != HttpStatusCode.Conflict && typeof(TOut) == typeof(TxnResponse)))
            {
                if (ResponseStream == null)
                    throw new ConsulRequestException($"Unexpected response, status code {response.StatusCode}",
                        response.StatusCode);
                using (var sr = new StreamReader(ResponseStream))
                {
                    throw new ConsulRequestException(
                        $"Unexpected response, status code {response.StatusCode}: {sr.ReadToEnd()}",
                        response.StatusCode);
                }
            }

            if (response.IsSuccessStatusCode ||
                // Special case for KV txn operations
                response.StatusCode == HttpStatusCode.Conflict && typeof(TOut) == typeof(TxnResponse))
                result.Response = Deserialize<TOut>(ResponseStream);

            result.RequestTime = Timer.Elapsed;
            Timer.Stop();

            return result;
        }

        protected override void ApplyOptions(ConsulClientConfiguration clientConfig)
        {
            if (Options == WriteOptions.Default) return;

            if (!string.IsNullOrEmpty(Options.Datacenter)) Params["dc"] = Options.Datacenter;
        }

        protected override void ApplyHeaders(HttpRequestMessage message, ConsulClientConfiguration clientConfig)
        {
            if (!string.IsNullOrEmpty(Options.Token)) message.Headers.Add("X-Consul-Token", Options.Token);
        }
    }

    public class PostRequest<TIn, TOut> : ConsulRequest
    {
        private readonly TIn _body;

        public PostRequest(ConsulClient.ConsulClient client, string url, TIn body, WriteOptions options = null) : base(
            client, url,
            HttpMethod.Post)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentException(nameof(url));
            _body = body;
            Options = options ?? WriteOptions.Default;
        }

        public WriteOptions Options { get; set; }

        public async Task<WriteResult<TOut>> Execute(CancellationToken ct)
        {
            Client.CheckDisposed();
            Timer.Start();
            var result = new WriteResult<TOut>();

            HttpContent content = null;

            if (typeof(TIn) == typeof(byte[]))
            {
                var bodyBytes = _body as byte[];
                if (bodyBytes != null) content = new ByteArrayContent(bodyBytes);
                // If body is null and should be a byte array, then just don't send anything.
            }
            else
            {
                content = new ByteArrayContent(Serialize(_body));
            }

            var message = new HttpRequestMessage(HttpMethod.Post, BuildConsulUri(Endpoint, Params));
            ApplyHeaders(message, Client.Config);
            message.Content = content;
            var response = await Client.HttpClient.SendAsync(message, ct).ConfigureAwait(false);

            result.StatusCode = response.StatusCode;

            ResponseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            if (response.StatusCode != HttpStatusCode.NotFound && !response.IsSuccessStatusCode)
            {
                if (ResponseStream == null)
                    throw new ConsulRequestException($"Unexpected response, status code {response.StatusCode}",
                        response.StatusCode);
                using (var sr = new StreamReader(ResponseStream))
                {
                    throw new ConsulRequestException(
                        $"Unexpected response, status code {response.StatusCode}: {sr.ReadToEnd()}",
                        response.StatusCode);
                }
            }

            if (response.IsSuccessStatusCode) result.Response = Deserialize<TOut>(ResponseStream);

            result.RequestTime = Timer.Elapsed;
            Timer.Stop();

            return result;
        }

        protected override void ApplyOptions(ConsulClientConfiguration clientConfig)
        {
            if (Options == WriteOptions.Default) return;

            if (!string.IsNullOrEmpty(Options.Datacenter)) Params["dc"] = Options.Datacenter;
        }

        protected override void ApplyHeaders(HttpRequestMessage message, ConsulClientConfiguration clientConfig)
        {
            if (!string.IsNullOrEmpty(Options.Token)) message.Headers.Add("X-Consul-Token", Options.Token);
        }
    }
}