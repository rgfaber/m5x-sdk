using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using M5x.DEC.Schema.Utils;
using Serilog;

namespace M5x.DEC.Infra.Web;

public static class Http
{
    public interface ISyncClient
    {
    }

    public interface IQueryClient<in TQuery, TResponse> : ISyncClient
        where TQuery : IQuery
        where TResponse : IResponse
    {
        Task<TResponse> Post(TQuery qry);
    }

    public abstract class QueryClient<TQuery, TResponse> : IQueryClient<TQuery, TResponse>
        where TQuery : IQuery
        where TResponse : IResponse, new()
    {
        private readonly HttpClient _http;
        private readonly ILogger _logger;

        protected QueryClient(IHttpFactory fact,
            ILogger logger,
            Authenticate.IClient authenticator = null)
        {
            _logger = logger;
            _http = fact.CreateClient(authenticator).Result;
        }

        private string Route => GetRoute();

        public async Task<TResponse> Post(TQuery qry)
        {
            using (_http)
            {
                var rsp = new TResponse();
                try
                {
                    var res = await _http.PostAsJsonAsync(Route, qry);
                    res.EnsureSuccessStatusCode();
                    rsp = await res.Content.ReadFromJsonAsync<TResponse>();
                }
                catch (Exception e)
                {
                    rsp.ErrorState.Errors.Add($"{GetType().PrettyPrint()}.WebError",
                        e.AsApiError());
                    _logger?.Debug($"{e.AsApiError()}");
                }

                return rsp;
            }
        }

        private string GetRoute()
        {
            var atts = (EndpointAttribute[])GetType().GetCustomAttributes(typeof(EndpointAttribute), true);
            if (atts.Length == 0) throw new Exception($"Attribute 'Route' is not defined on {GetType()}!");
            return atts[0].Endpoint;
        }
    }


    public interface IHopeClient<in THope, TFeedback> : ISyncClient
        where THope : IHope
        where TFeedback : IFeedback
    {
        Task<TFeedback> Post(THope hope);
    }

    public abstract class HopeClient<THope, TFeedback> : IHopeClient<THope, TFeedback>
        where THope : IHope
        where TFeedback : IFeedback, new()
    {
        private readonly HttpClient _http;
        private readonly ILogger _logger;

        protected HopeClient(
            IHttpFactory fact,
            ILogger logger,
            Authenticate.IClient authenticator = null)
        {
            _logger = logger;
            _http = fact.CreateClient(authenticator).Result;
        }

        private string Route => GetRoute();

        public async Task<TFeedback> Post(THope hope)
        {
            using (_http)
            {
                var rsp = new TFeedback();
                try
                {
                    var res = await _http.PostAsJsonAsync(Route, hope);
                    res.EnsureSuccessStatusCode();
                    rsp = await res.Content.ReadFromJsonAsync<TFeedback>();
                }
                catch (Exception e)
                {
                    _logger?.Debug($"{e.AsApiError()}");
                    rsp.ErrorState.Errors.Add($"{GetType().PrettyPrint()}.WebError",
                        e.AsApiError());
                }

                return rsp;
            }
        }

        private string GetRoute()
        {
            var atts = (EndpointAttribute[])GetType().GetCustomAttributes(typeof(EndpointAttribute), true);
            if (atts.Length == 0) throw new Exception($"Attribute 'Route' is not defined on {GetType()}!");
            return atts[0].Endpoint;
        }
    }


    public interface IHttpOptions
    {
        string BaseUrl { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        int Timeout { get; set; }
    }

    public record HttpOptions : IHopeOptions, IQueryOptions
    {
        public string BaseUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Timeout { get; set; }
    }


    public interface IHopeOptions : IHttpOptions
    {
    }


    public interface IQueryOptions : IHttpOptions
    {
    }


    public interface IHttpFactory
    {
        Task<HttpClient> CreateClient(Authenticate.IClient authenticator = null);
    }

    public abstract class HttpFactory : IHttpFactory
    {
        private readonly Authenticate.IClient _authenticator;
        private readonly IHttpOptions _httpOptions;
        private readonly ILogger _logger;

        protected HttpFactory(IHttpOptions httpOptions, Authenticate.IClient authenticator = null)
        {
            _httpOptions = httpOptions;
            _authenticator = authenticator;
        }

        public async Task<HttpClient> CreateClient(Authenticate.IClient authenticator = null)
        {
            var clt = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(_httpOptions.Timeout),
                BaseAddress = new Uri(_httpOptions.BaseUrl)
            };
            if (authenticator == null) return clt;
            if (Authenticate.SessionCache.Default.SessionToken != GuidUtils.NullCleanGuid &&
                !string.IsNullOrWhiteSpace(Authenticate.SessionCache.Default.SessionToken)) return clt;
            var authRsp = await authenticator.Authenticate(_httpOptions.UserName, _httpOptions.Password);
            if (authRsp.ErrorState.IsSuccessful)
                clt.DefaultRequestHeaders.Add("Authorization",
                    "Bearer " + Authenticate.SessionCache.Default.SessionToken);
            return clt;
        }
    }
}