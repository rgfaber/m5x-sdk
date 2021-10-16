using System;
using System.Threading.Tasks;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Authentication;
using M5x.DEC.Schema.Utils;
using Serilog;

namespace M5x.DEC.Infra.Web
{
    public static class Authenticate
    {
        public class SessionCache
        {
            private static SessionCache _default;
            private Feedback _feedback;

            private Hope _hope;

            public static SessionCache Default => _default ??= new SessionCache();

            public string SessionToken => _feedback?.Payload.Token;

            public void StoreSession(Hope hope, Feedback result)
            {
                _hope = hope;
                _feedback = result;
            }
        }


        public record Hope : Hope<UserCredentials>
        {
            public Hope()
            {
            }

            public Hope(string aggregateId, string correlationId) : base(aggregateId, correlationId)
            {
            }

            public Hope(string correlationId, UserCredentials payload) : base(Guid.NewGuid().ToString(), correlationId)
            {
                Payload = payload;
            }

            public static Hope New(string correlationId, UserCredentials credentials)
            {
                return new Hope(correlationId, credentials);
            }
        }

        public record Feedback : Feedback<AuthInfo>
        {
        }


        public interface IClient : Http.IHopeClient<Hope, Feedback>
        {
            Task<Feedback> Authenticate(string username, string password);
        }


        internal class Client : Http.HopeClient<Hope, Feedback>, IClient
        {
            private readonly IClient _authenticator;
            private readonly Http.IHttpFactory _fact;
            private readonly Http.HttpOptions _hopeOptions;
            private readonly ILogger _logger;

            public Client(Http.IHttpFactory fact,
                ILogger logger,
                IClient authenticator = null) : base(fact, logger, authenticator)
            {
                _fact = fact;
                _logger = logger;
                _authenticator = authenticator;
            }

            public async Task<Feedback> Authenticate(string username, string password)
            {
                var cred = UserCredentials.New(username, password);
                var req = Hope.New(GuidUtils.NewCleanGuid, cred);
                var rsp = await Post(req);
                if (rsp.ErrorState.IsSuccessful)
                    SessionCache.Default.StoreSession(req, rsp);
                return rsp;
            }
        }
    }
}