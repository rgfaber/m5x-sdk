using System.Threading.Tasks;
using M5x.DEC.Infra.Web;
using M5x.DEC.Schema;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration.Client
{
    public abstract class HopeClientTests<TClient, THope, TFeedback> : ConnectedTests<TClient>
        where TClient : Http.IHopeClient<THope, TFeedback>
        where THope : IHope
        where TFeedback : IFeedback
    {
        protected TClient _client;

        protected HopeClientTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        [Fact]
        public Task Needs_Client()
        {
            Assert.NotNull(_client);
            return Task.CompletedTask;
        }

        [Fact]
        public async Task Must_ReturnFeedback()
        {
            var hope = CreateHope();
            var res = await _client.Post(hope);
            Assert.NotNull(res);
            Assert.IsType<TFeedback>(res);
        }

        protected abstract THope CreateHope();
    }
}