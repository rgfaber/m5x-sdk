using System;
using System.Net.Http;
using System.Net.Http.Headers;
using M5x.Consul.ACL;
using M5x.Consul.Client;
using M5x.Consul.KV;
using Newtonsoft.Json;

namespace M5x.Consul.ConsulClient
{
    /// <summary>
    ///     Represents a persistant connection to a Consul agent. Instances of this class should be created rarely and reused
    ///     often.
    /// </summary>
    public partial class ConsulClient : IDisposable
    {
        private readonly ConsulClientConfigurationContainer _configContainer;

        internal readonly JsonSerializer Serializer = new();

        public HttpClient HttpClient => _configContainer.HttpClient;

        public HttpClientHandler HttpHandler => _configContainer.HttpHandler;
        public ConsulClientConfiguration Config => _configContainer.Config;

        private void InitializeEndpoints()
        {
            _acl = new Lazy<Acl>(() => new Acl(this));
            _agent = new Lazy<Agent.Agent>(() => new Agent.Agent(this));
            _catalog = new Lazy<Catalog.Catalog>(() => new Catalog.Catalog(this));
            _coordinate = new Lazy<Coordinate.Coordinate>(() => new Coordinate.Coordinate(this));
            _event = new Lazy<Event.Event>(() => new Event.Event(this));
            _health = new Lazy<Health.Health>(() => new Health.Health(this));
            _kv = new Lazy<Kv>(() => new Kv(this));
            _operator = new Lazy<Operator.Operator>(() => new Operator.Operator(this));
            _preparedquery = new Lazy<PreparedQuery.PreparedQuery>(() => new PreparedQuery.PreparedQuery(this));
            _raw = new Lazy<Raw.Raw>(() => new Raw.Raw(this));
            _session = new Lazy<Session.Session>(() => new Session.Session(this));
            _snapshot = new Lazy<Snapshot.Snapshot>(() => new Snapshot.Snapshot(this));
            _status = new Lazy<Status.Status>(() => new Status.Status(this));
        }

        private void HandleConfigUpdateEvent(object sender, EventArgs e)
        {
            ApplyConfig(sender as ConsulClientConfiguration, HttpHandler, HttpClient);
        }

        private void ApplyConfig(ConsulClientConfiguration config, HttpClientHandler handler, HttpClient client)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            if (config.HttpAuth != null)
#pragma warning restore CS0618 // Type or member is obsolete
            {
#pragma warning disable CS0618 // Type or member is obsolete
                handler.Credentials = config.HttpAuth;
#pragma warning restore CS0618 // Type or member is obsolete
            }
#if !__MonoCS__
            if (config.ClientCertificateSupported)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                if (config.ClientCertificate != null)
#pragma warning restore CS0618 // Type or member is obsolete
                {
                    handler.ClientCertificateOptions = ClientCertificateOption.Manual;
#pragma warning disable CS0618 // Type or member is obsolete
                    handler.ClientCertificates.Add(config.ClientCertificate);
#pragma warning restore CS0618 // Type or member is obsolete
                }
                else
                {
                    handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                    handler.ClientCertificates.Clear();
                }
            }
#endif
#pragma warning disable CS0618 // Type or member is obsolete
            if (config.DisableServerCertificateValidation)
#pragma warning restore CS0618 // Type or member is obsolete
                handler.ServerCertificateCustomValidationCallback += (certSender, cert, chain, sslPolicyErrors) => true;
            else
                handler.ServerCertificateCustomValidationCallback = null;

            if (!string.IsNullOrEmpty(config.Token))
            {
                if (client.DefaultRequestHeaders.Contains("X-Consul-Token"))
                    client.DefaultRequestHeaders.Remove("X-Consul-Token");
                client.DefaultRequestHeaders.Add("X-Consul-Token", config.Token);
            }
        }

        public GetRequest<TOut> Get<TOut>(string path, QueryOptions opts = null)
        {
            return new GetRequest<TOut>(this, path, opts ?? QueryOptions.Default);
        }

        internal DeleteReturnRequest<TOut> DeleteReturning<TOut>(string path, WriteOptions opts = null)
        {
            return new DeleteReturnRequest<TOut>(this, path, opts ?? WriteOptions.Default);
        }

        internal DeleteRequest Delete(string path, WriteOptions opts = null)
        {
            return new DeleteRequest(this, path, opts ?? WriteOptions.Default);
        }

        internal DeleteAcceptingRequest<TIn> DeleteAccepting<TIn>(string path, TIn body, WriteOptions opts = null)
        {
            return new DeleteAcceptingRequest<TIn>(this, path, body, opts ?? WriteOptions.Default);
        }

        internal PutReturningRequest<TOut> PutReturning<TOut>(string path, WriteOptions opts = null)
        {
            return new PutReturningRequest<TOut>(this, path, opts ?? WriteOptions.Default);
        }

        public PutRequest<TIn> Put<TIn>(string path, TIn body, WriteOptions opts = null)
        {
            return new PutRequest<TIn>(this, path, body, opts ?? WriteOptions.Default);
        }

        internal PutNothingRequest PutNothing(string path, WriteOptions opts = null)
        {
            return new PutNothingRequest(this, path, opts ?? WriteOptions.Default);
        }

        internal PutRequest<TIn, TOut> Put<TIn, TOut>(string path, TIn body, WriteOptions opts = null)
        {
            return new PutRequest<TIn, TOut>(this, path, body, opts ?? WriteOptions.Default);
        }

        internal PostRequest<TIn> Post<TIn>(string path, TIn body, WriteOptions opts = null)
        {
            return new PostRequest<TIn>(this, path, body, opts ?? WriteOptions.Default);
        }

        internal PostRequest<TIn, TOut> Post<TIn, TOut>(string path, TIn body, WriteOptions opts = null)
        {
            return new PostRequest<TIn, TOut>(this, path, body, opts ?? WriteOptions.Default);
        }

        /// <summary>
        ///     This class is used to group all the configurable bits of a ConsulClient into a single pointer reference
        ///     which is great for implementing reconfiguration later.
        /// </summary>
        private class ConsulClientConfigurationContainer
        {
            public readonly ConsulClientConfiguration Config;
            internal readonly HttpClient HttpClient;

            internal readonly HttpClientHandler HttpHandler;
            internal readonly bool SkipClientDispose;

            public ConsulClientConfigurationContainer()
            {
                Config = new ConsulClientConfiguration();
                HttpHandler = new HttpClientHandler();
                HttpClient = new HttpClient(HttpHandler) { Timeout = TimeSpan.FromMinutes(15) };
                HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpClient.DefaultRequestHeaders.Add("Keep-Alive", "true");
            }

            #region Old style config handling

            public ConsulClientConfigurationContainer(ConsulClientConfiguration config, HttpClient client)
            {
                SkipClientDispose = true;
                Config = config;
                HttpClient = client;
            }

            public ConsulClientConfigurationContainer(ConsulClientConfiguration config)
            {
                Config = config;
                HttpHandler = new HttpClientHandler();
                HttpClient = new HttpClient(HttpHandler);
                HttpClient.Timeout = TimeSpan.FromMinutes(15);
                HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpClient.DefaultRequestHeaders.Add("Keep-Alive", "true");
            }

            #endregion

            #region IDisposable Support

            private bool _disposedValue; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!_disposedValue)
                {
                    if (disposing)
                    {
                        if (HttpClient != null && !SkipClientDispose) HttpClient.Dispose();
                        if (HttpHandler != null) HttpHandler.Dispose();
                    }

                    _disposedValue = true;
                }
            }

            //~ConsulClient()
            //{
            //    // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            //    Dispose(false);
            //}

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            public void CheckDisposed()
            {
                if (_disposedValue)
                    throw new ObjectDisposedException(typeof(ConsulClientConfigurationContainer).FullName);
            }

            #endregion
        }

        #region New style config with Actions

        /// <summary>
        ///     Initializes a new Consul client with a default configuration that connects to 127.0.0.1:8500.
        /// </summary>
        public ConsulClient() : this(null, null, null)
        {
        }

        /// <summary>
        ///     Initializes a new Consul client with the ability to set portions of the configuration.
        /// </summary>
        /// <param name="configOverride">The Action to modify the default configuration with</param>
        public ConsulClient(Action<ConsulClientConfiguration> configOverride) : this(configOverride, null, null)
        {
        }

        /// <summary>
        ///     Initializes a new Consul client with the ability to set portions of the configuration and access the underlying
        ///     HttpClient for modification.
        ///     The HttpClient is modified to set options like the request timeout and headers.
        ///     The Timeout property also applies to all long-poll requests and should be set to a value that will encompass all
        ///     successful requests.
        /// </summary>
        /// <param name="configOverride">The Action to modify the default configuration with</param>
        /// <param name="clientOverride">The Action to modify the HttpClient with</param>
        public ConsulClient(Action<ConsulClientConfiguration> configOverride, Action<HttpClient> clientOverride) : this(
            configOverride, clientOverride, null)
        {
        }

        /// <summary>
        ///     Initializes a new Consul client with the ability to set portions of the configuration and access the underlying
        ///     HttpClient and WebRequestHandler for modification.
        ///     The HttpClient is modified to set options like the request timeout and headers.
        ///     The WebRequestHandler is modified to set options like Proxy and Credentials.
        ///     The Timeout property also applies to all long-poll requests and should be set to a value that will encompass all
        ///     successful requests.
        /// </summary>
        /// <param name="configOverride">The Action to modify the default configuration with</param>
        /// <param name="clientOverride">The Action to modify the HttpClient with</param>
        /// <param name="handlerOverride">The Action to modify the WebRequestHandler with</param>
        public ConsulClient(Action<ConsulClientConfiguration> configOverride, Action<HttpClient> clientOverride,
            Action<HttpClientHandler> handlerOverride)
        {
            var ctr = new ConsulClientConfigurationContainer();

            configOverride?.Invoke(ctr.Config);
            ApplyConfig(ctr.Config, ctr.HttpHandler, ctr.HttpClient);
            handlerOverride?.Invoke(ctr.HttpHandler);
            clientOverride?.Invoke(ctr.HttpClient);

            _configContainer = ctr;

            InitializeEndpoints();
        }

        #endregion

        #region Old style config

        /// <summary>
        ///     Initializes a new Consul client with the configuration specified.
        /// </summary>
        /// <param name="config">A Consul client configuration</param>
        [Obsolete(
            "This constructor is no longer necessary due to the new Action based constructors and will be removed when 0.8.0 is released." +
            "Please use the ConsulClient(Action<ConsulClientConfiguration>) constructor to set configuration options.",
            false)]
        public ConsulClient(ConsulClientConfiguration config)
        {
            config.Updated += HandleConfigUpdateEvent;
            var ctr = new ConsulClientConfigurationContainer(config);
            ApplyConfig(ctr.Config, ctr.HttpHandler, ctr.HttpClient);

            _configContainer = ctr;
            InitializeEndpoints();
        }

        /// <summary>
        ///     Initializes a new Consul client with the configuration specified and a custom HttpClient, which is useful for
        ///     setting proxies/custom timeouts.
        ///     The HttpClient must accept the "application/json" content type and the Timeout property should be set to at least
        ///     15 minutes to allow for blocking queries.
        /// </summary>
        /// <param name="config">A Consul client configuration</param>
        /// <param name="client">A custom HttpClient</param>
        [Obsolete(
            "This constructor is no longer necessary due to the new Action based constructors and will be removed when 0.8.0 is released." +
            "Please use one of the ConsulClient(Action<>) constructors instead to set internal options on the HttpClient/WebRequestHandler.",
            false)]
        public ConsulClient(ConsulClientConfiguration config, HttpClient client)
        {
            var ctr = new ConsulClientConfigurationContainer(config, client);
            if (!ctr.HttpClient.DefaultRequestHeaders.Accept.Contains(
                new MediaTypeWithQualityHeaderValue("application/json")))
                throw new ArgumentException("HttpClient must accept the application/json content type", nameof(client));
            _configContainer = ctr;
            InitializeEndpoints();
        }

        #endregion

        #region IDisposable Support

        private bool _disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Config.Updated -= HandleConfigUpdateEvent;
                    if (_configContainer != null) _configContainer.Dispose();
                }

                _disposedValue = true;
            }
        }

        //~ConsulClient()
        //{
        //    // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //    Dispose(false);
        //}

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void CheckDisposed()
        {
            if (_disposedValue) throw new ObjectDisposedException(typeof(ConsulClient).FullName);
        }

        #endregion
    }
}