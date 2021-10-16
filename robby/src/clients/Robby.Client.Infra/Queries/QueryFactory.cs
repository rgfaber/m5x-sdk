using M5x.DEC.Infra.Web;

namespace Robby.Client.Infra.Queries
{
    public interface IQueryFactory : Http.IHttpFactory
    {
    }

    internal class QueryFactory : Http.HttpFactory, IQueryFactory
    {
        public QueryFactory(Http.IQueryOptions httpOptions, Authenticate.IClient authenticator = null) : base(
            httpOptions, authenticator)
        {
        }
    }
}