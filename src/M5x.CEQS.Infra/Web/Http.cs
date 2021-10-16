using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EventFlow.Extensions;
using M5x.CEQS.Schema;
using M5x.CEQS.Schema.Extensions;
using M5x.CEQS.Schema.Utils;
using Serilog;

namespace M5x.CEQS.Infra.Web
{
    public static class Http
    {
        public interface IQueryClient<in TQuery, TResponse>
            where TQuery : IQuery
            where TResponse : IResponse
        {
            Task<TResponse> Post(TQuery req);
        }
        
        public abstract class QueryClient<TQuery, TResponse> : IQueryClient<TQuery, TResponse> 
            where TQuery : IQuery 
            where TResponse : IResponse, new()
        {
            private readonly IClientOptions _clientOptions;
            private readonly ILogger _logger;
            private readonly HttpClient _http;

            private string Route => GetRoute();
        
            private string GetRoute()
            {
                var atts = (RouteAttribute[]) GetType().GetCustomAttributes(typeof(RouteAttribute), true);
                if (atts.Length == 0) throw new Exception($"Attribute 'Route' is not defined on {GetType()}!");
                return atts[0].Route;
            }

            protected QueryClient(IHttpFactory fact, 
                IClientOptions clientOptions, 
                ILogger logger,
                Authenticate.IClient authenticator=null)
            {
                _clientOptions = clientOptions;
                _logger = logger;
                _http =  fact.CreateClient(authenticator).Result;
            }

            public async Task<TResponse> Post(TQuery req)
            {
                using (_http)
                {
                    var rsp = new TResponse();
                    try
                    {
                        var res = await _http.PostAsJsonAsync<TQuery>(Route, req);
                        res.EnsureSuccessStatusCode();
                        rsp = await res.Content.ReadFromJsonAsync<TResponse>();
                    }
                    catch (Exception e)
                    {
                        rsp.ErrorState.Errors.Add($"{TypeExtensions.PrettyPrint(GetType())}.WebError",
                            ExceptionExtensions.AsApiError(e));
                        _logger?.Debug($"{e.AsApiError()}");
                    }

                    return rsp;
                }
            }

        }
        

        
        
        
        
        public interface IHopeClient<in THope, TFeedback>
            where THope : IHope
            where TFeedback : IFeedback
        {
            Task<TFeedback> Post(THope req);
        }
    
        public abstract class HopeClient<THope, TFeedback> : IHopeClient<THope, TFeedback> 
            where THope : IHope 
            where TFeedback : IFeedback, new()
        {
            private readonly IClientOptions _clientOptions;
            private readonly ILogger _logger;
            private readonly HttpClient _http;

            private string Route => GetRoute();
        
            private string GetRoute()
            {
                var atts = (RouteAttribute[]) GetType().GetCustomAttributes(typeof(RouteAttribute), true);
                if (atts.Length == 0) throw new Exception($"Attribute 'Route' is not defined on {GetType()}!");
                return atts[0].Route;
            }

            protected HopeClient(IHttpFactory fact, 
                IClientOptions clientOptions, 
                ILogger logger,
                Authenticate.IClient authenticator=null)
            {
                _clientOptions = clientOptions;
                _logger = logger;
                _http =  fact.CreateClient(authenticator).Result;
            }

            public async Task<TFeedback> Post(THope req)
            {
                using (_http)
                {
                    var rsp = new TFeedback();
                    try
                    {
                        var res = await _http.PostAsJsonAsync<THope>(Route, req);
                        res.EnsureSuccessStatusCode();
                        rsp = await res.Content.ReadFromJsonAsync<TFeedback>();
                    }
                    catch (Exception e)
                    {
                        _logger?.Debug($"{e.AsApiError()}");
                        rsp.ErrorState.Errors.Add($"{TypeExtensions.PrettyPrint(GetType())}.WebError",
                            ExceptionExtensions.AsApiError(e)); 
                    }

                    return rsp;
                }
            }
        }


        public interface IClientOptions
        {
            string BaseUrl { get; set; }
            string UserName { get; set; }
            string Password { get; set; }
            int Timeout { get; set; }
        }

        public record ClientOptions : IClientOptions
        {
            public string BaseUrl { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public int Timeout { get; set; }
        }
        
        
        public interface IHttpFactory
        {
            Task<HttpClient> CreateClient(Authenticate.IClient authenticator = null);
        }
        
        internal class HttpFactory : IHttpFactory
        {
            private readonly IClientOptions _clientOptions;
            private readonly Authenticate.IClient _authenticator;
            private readonly ILogger _logger;

            public HttpFactory(IClientOptions clientOptions, Authenticate.IClient authenticator = null)
            {
                _clientOptions = clientOptions;
                _authenticator = authenticator;
            }

            protected virtual void SetupClientJwt(HttpClient clt)
            {
                clt.DefaultRequestHeaders.Add("Authorization", "Bearer " + Authenticate.SessionCache.Default.SessionToken);
            }

            protected virtual void SetupClientDefaults(HttpClient client)
            {
                client.Timeout = TimeSpan.FromSeconds(_clientOptions.Timeout); //set your own timeout.
                client.BaseAddress = new Uri(_clientOptions.BaseUrl);
            }

            public async Task<HttpClient> CreateClient(Authenticate.IClient authenticator = null)
            {
                var res = new HttpClient();
                SetupClientDefaults(res);
                if (authenticator == null) return res;
                if (Authenticate.SessionCache.Default.SessionToken != GuidUtils.NullGuid &&
                    !string.IsNullOrWhiteSpace(Authenticate.SessionCache.Default.SessionToken)) return res;
                var authRsp = await authenticator.Authenticate(_clientOptions.UserName, _clientOptions.Password);
                if (authRsp.ErrorState.IsSuccessful)
                {
                    SetupClientJwt(res);
                }
                return res;
            }
        }
    }
}