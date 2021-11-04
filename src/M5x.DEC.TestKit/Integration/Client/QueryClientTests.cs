using System.Threading.Tasks;
using M5x.DEC.Infra.Web;
using M5x.DEC.Schema;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration.Client
{
    public abstract class QueryClientTests<TClient, TQuery, TResponse> : ConnectedTests<TClient>
        where TClient : Http.IQueryClient<TQuery, TResponse>
        where TQuery : IQuery
        where TResponse : IResponse
    {
        protected TClient _client;

        protected QueryClientTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        [Fact]
        public Task Needs_Client()
        {
            Assert.NotNull(_client);
            return Task.CompletedTask;
        }

        [Fact]
        public Task Must_ReturnResponse()
        {
            var qry = CreateQuery();
            var res = _client.Post(qry).Result;
            Assert.NotNull(res);
            Assert.IsType<TResponse>(res);
            return Task.CompletedTask;
        }

        protected abstract TQuery CreateQuery();
    }
}