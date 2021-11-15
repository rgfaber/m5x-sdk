using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EventStore.Client;
using M5x.Config;
using M5x.EventStore.Interfaces;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace M5x.EventStore.Tests
{
    public class Tested
    {
        public bool IsTested { get; set; }
        public string Name { get; set; }
    }

    public class EsClientTests : IoCTestsBase
    {
        private IEsClient _clt;


        public EsClientTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }


        [Fact]
        public async Task Should_AppendToStream()
        {
            var ev = new Tested
            {
                IsTested = false,
                Name = "Raf"
            };
            var _uuid = Uuid.FromGuid(Guid.NewGuid());
            var t = typeof(Tested).AssemblyQualifiedName;
            var d = Serialize(ev);

            var m = Encoding.UTF8.GetBytes("{}");

            var data = new EventData(_uuid, t, d, m);
            var res = await _clt.AppendToStreamAsync("TestStream",
                StreamState.Any,
                new[] { data });
            ;
            Assert.NotNull(res);
        }

        [Fact]
        public void Try_EncodingGetBytes()
        {
            var d = Encoding.UTF8.GetBytes("{}");
            Assert.NotEmpty(d);
        }


        private byte[] Serialize(Tested @event)
        {
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));
        }

        protected override void Initialize()
        {
            _clt = Container.GetService<IEsClient>();
        }

        [Fact]
        public void Needs_Client()
        {
            Assert.NotNull(_clt);
        }

        protected override void SetTestEnvironment()
        {
            DotEnv.FromEmbedded();
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
            services.AddDECEsClients();
        }
    }
}