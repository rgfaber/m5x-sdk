using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using M5x.Schemas;
using Serilog;

namespace M5x.DEC.Infra.Web
{
    public static class Authenticate
    {
        
        public class SessionCache
        {
            private static SessionCache _default;

            private Req _req;
            private Rsp _rsp;

            public static SessionCache Default => _default ??= new SessionCache();

            public string SessionToken => _rsp?.Token;

            public void StoreSession(Req req, Rsp result)
            {
                _req = req;
                _rsp = result;
            }
        }
        

        public static class Config
        {
            
        }
        
        public record Req : Request
        {
            public Req(string correlationId, string username, string password) : base(correlationId)
            {
                Username = username;
                Password = password;
            }

            [Required] public string Username { get; set; }

            [Required] public string Password { get; set; }

            public static Req CreateNew(string correlationId, string username, string password)
            {
                return new(correlationId, username, password);
            }
        }
        
        public record Rsp : Response
        {
            public string Token { get; set; }
            public string Username { get; set; }
            public string DisplayName { get; set; }
            public string UserId { get; set; }
        }
        
        public interface IClient : Http.IRestClient<Req,Rsp>
        {
            Task<Rsp> Authenticate(string username, string password);
        }
        
        
        internal class Client : Http.RestClient<Authenticate.Req, Authenticate.Rsp>, IClient
        {
            private readonly Http.IHttpFactory _fact;
            private readonly Http.ClientOptions _clientOptions;
            private readonly ILogger _logger;
            private readonly IClient _authenticator;

            public Client(Http.IHttpFactory fact, 
                Http.ClientOptions clientOptions, 
                ILogger logger, 
                IClient authenticator = null) : base(fact, clientOptions, logger, authenticator)
            {
                _fact = fact;
                _clientOptions = clientOptions;
                _logger = logger;
                _authenticator = authenticator;
            }

            public async Task<Rsp> Authenticate(string username, string password)
            {
                var req = Req.CreateNew( GuidFactories.NewCleanGuid, username, password);
                var rsp = await Post(req);
                if(rsp.ErrorState.IsSuccessful)
                    SessionCache.Default.StoreSession(req, rsp);
                return rsp;
            }
        }
        
    }
}