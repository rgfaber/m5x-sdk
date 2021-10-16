using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using M5x.Schemas;
using M5x.Schemas.Extensions;
using Serilog;

namespace M5x.DEC.Infra.Web
{
    public static class Http
    {
        
        public interface IRestClient<TReq, TRsp>
            where TReq : IRequest
            where TRsp : IResponse
        {
            Task<TRsp> Post(TReq req);
            Task<TRsp> Put(TReq req);
            Task<TRsp> Get(TReq req, string requestUri);
        }
    
        public abstract class RestClient<TReq, TRsp> : IRestClient<TReq, TRsp> 
            where TReq : IRequest 
            where TRsp : IResponse, new()
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

            protected RestClient(IHttpFactory fact, 
                IClientOptions clientOptions, 
                ILogger logger,
                Authenticate.IClient authenticator=null)
            {
                _clientOptions = clientOptions;
                _logger = logger;
                _http =  fact.CreateClient(authenticator).Result;
            }

            public async Task<TRsp> Post(TReq req)
            {
                using (_http)
                {
                    var rsp = new TRsp();
                    try
                    {
                        var res = await _http.PostAsJsonAsync<TReq>(Route, req);
                        res.EnsureSuccessStatusCode();
                        rsp = await res.Content.ReadFromJsonAsync<TRsp>();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        rsp.ErrorState.Errors.Add($"{GetType().PrettyPrint()}.WebError", e.AsApiError());
                    }

                    return rsp;
                }
            }

            public async Task<TRsp> Put(TReq req)
            {
                using (_http)
                {
                    var rsp = new TRsp();
                    try
                    {
                        var res = await _http.PutAsJsonAsync(Route, req);
                        res.EnsureSuccessStatusCode();
                        rsp = await res.Content.ReadFromJsonAsync<TRsp>();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        rsp.ErrorState.Errors.Add($"{GetType().PrettyPrint()}.WebError", e.AsApiError());
                    }

                    return rsp;
                }
            }

            public async Task<TRsp> Get(TReq req, string requestUri)
            {
                using (_http)
                {
                    var rsp = new TRsp();
                    try
                    {
                        rsp = await _http.GetFromJsonAsync<TRsp>(requestUri);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        rsp.ErrorState.Errors.Add($"{GetType().PrettyPrint()}.WebError", e.AsApiError());
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
                if (Authenticate.SessionCache.Default.SessionToken != GuidFactories.NullGuid &&
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