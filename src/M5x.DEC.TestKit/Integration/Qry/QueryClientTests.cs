using System.Threading.Tasks;
using M5x.DEC.Infra.Web;
using M5x.DEC.Schema;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration.Qry
{
    public abstract class QueryClientTests<TClient, TQuery, TResponse> : IoCTestsBase
        where TClient : Http.IQueryClient<TQuery, TResponse>
        where TQuery : IQuery
        where TResponse : IResponse
    {
        protected TClient _client;

        protected QueryClientTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        [Fact]
        public void Needs_Client()
        {
            Assert.NotNull(_client);
        }

        [Fact]
        public async Task Must_ReturnResponse()
        {
            var qry = CreateQuery();
            var res = await _client.Post(qry);
            Assert.NotNull(res);
            Assert.IsType<TResponse>(res);
        }

        protected abstract TQuery CreateQuery();
    }
}