using M5x.DEC.Infra.Web;

namespace Robby.Client.Infra.Features
{
    public interface IHopeFactory : Http.IHttpFactory
    {
    }

    internal class HopeFactory : Http.HttpFactory, IHopeFactory
    {
        public HopeFactory(Http.IHopeOptions httpOptions, Authenticate.IClient authenticator = null) : base(httpOptions,
            authenticator)
        {
        }
    }
}