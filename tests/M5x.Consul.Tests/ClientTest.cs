using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using M5x.Consul.Client;
using M5x.Consul.KV;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Consul.Tests
{
    public class ClientTest : ConsulTestsBase, IDisposable
    {
        public ClientTest(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        [Fact]
        public void Client_Constructors()
        {
            Action<ConsulClientConfiguration> cfgAction2 = cfg => { cfg.Token = "yep"; };
            Action<ConsulClientConfiguration> cfgAction = cfg =>
            {
                cfg.Datacenter = "foo";
                cfgAction2(cfg);
            };

            using (var c = new ConsulClient.ConsulClient())
            {
                Assert.NotNull(c.Config);
                Assert.NotNull(c.HttpHandler);
                Assert.NotNull(c.HttpClient);
            }

            using (var c = new ConsulClient.ConsulClient(cfgAction))
            {
                Assert.NotNull(c.Config);
                Assert.NotNull(c.HttpHandler);
                Assert.NotNull(c.HttpClient);
                Assert.Equal("foo", c.Config.Datacenter);
            }

            using (var c = new ConsulClient.ConsulClient(cfgAction,
                client => { client.Timeout = TimeSpan.FromSeconds(30); }))
            {
                Assert.NotNull(c.Config);
                Assert.NotNull(c.HttpHandler);
                Assert.NotNull(c.HttpClient);
                Assert.Equal("foo", c.Config.Datacenter);
                Assert.Equal(TimeSpan.FromSeconds(30), c.HttpClient.Timeout);
            }
        }

        [Fact]
        public async Task Client_CustomHttpClient()
        {
            using (var hc = new HttpClient())
            {
                hc.Timeout = TimeSpan.FromDays(10);
                hc.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
#pragma warning disable CS0618 // Type or member is obsolete
                using (var client = new ConsulClient.ConsulClient(new ConsulClientConfiguration(), hc))
#pragma warning restore CS0618 // Type or member is obsolete
                {
                    await client.KV.Put(new KVPair("customhttpclient") {Value = Encoding.UTF8.GetBytes("hello world")});
                    Assert.Equal(TimeSpan.FromDays(10), client.HttpClient.Timeout);
                    Assert.Contains(new MediaTypeWithQualityHeaderValue("application/json"),
                        client.HttpClient.DefaultRequestHeaders.Accept);
                }

                Assert.Equal("hello world",
                    await (await hc.GetAsync("http://localhost:8500/v1/kv/customhttpclient?raw")).Content
                        .ReadAsStringAsync());
            }
        }

//        [Fact]
//        public void Client_DefaultConfig_env()
//        {
//            const string addr = "http://127.0.0.1:8500";
//            const string token = "abcd1234";
//            const string auth = "username:password";
//            Environment.SetEnvironmentVariable("CONSUL_HTTP_ADDR", addr);
//            Environment.SetEnvironmentVariable("CONSUL_HTTP_TOKEN", token);
//            Environment.SetEnvironmentVariable("CONSUL_HTTP_AUTH", auth);
//            Environment.SetEnvironmentVariable("CONSUL_HTTP_SSL", "1");
//            Environment.SetEnvironmentVariable("CONSUL_HTTP_SSL_VERIFY", "0");

//            var client = new ConsulClient.ConsulClient();
//            var config = client.Config;

//            Assert.Equal(addr, $"{config.Address.Scheme}//{config.Address.Host}:{config.Address.Port}");
//            Assert.Equal(token, config.Token);
//#pragma warning disable CS0618 // Type or member is obsolete
//            Assert.NotNull(config.HttpAuth);
//            Assert.Equal("username", config.HttpAuth.UserName);
//            Assert.Equal("password", config.HttpAuth.Password);
//#pragma warning restore CS0618 // Type or member is obsolete
//            Assert.Equal("https", config.Address.Scheme);

//            Environment.SetEnvironmentVariable(EnVars.CONSUL_HTTP_ADDRESS, string.Empty);
//            Environment.SetEnvironmentVariable(EnVars.CONSUL_HTTP_TOKEN, string.Empty);
//            Environment.SetEnvironmentVariable(EnVars.CONSUL_HTTP_AUTH, string.Empty);
//            Environment.SetEnvironmentVariable(EnVars.CONSUL_HTTP_SSL, string.Empty);
//            Environment.SetEnvironmentVariable(EnVars.CONSUL_HTTP_SSL_VERIFY, string.Empty);


//#if !CORECLR
//            Assert.True((client.HttpHandler as WebRequestHandler).ServerCertificateValidationCallback(null, null, null,
//                SslPolicyErrors.RemoteCertificateChainErrors));
//            ServicePointManager.ServerCertificateValidationCallback = null;
//#else
//            Assert.True(client.HttpHandler.ServerCertificateCustomValidationCallback(null, null,
//                null,
//                SslPolicyErrors.RemoteCertificateChainErrors));
//#endif

//            Assert.NotNull(client);
//        }

        [Fact]
        public async Task Client_DisposeBehavior()
        {
            var client = new ConsulClient.ConsulClient();
            var opts = new WriteOptions
            {
                Datacenter = "foo",
                Token = "12345"
            };

            client.Dispose();

            var request = client.Put("/v1/kv/foo", new KVPair("kv/foo"), opts);

            await Assert.ThrowsAsync<ObjectDisposedException>(() => request.Execute(CancellationToken.None));
        }

        [Fact]
        public async Task Client_ReuseAndUpdateConfig()
        {
            var config = new ConsulClientConfiguration();

#pragma warning disable CS0618 // Type or member is obsolete
            using (var client = new ConsulClient.ConsulClient(config))
#pragma warning restore CS0618 // Type or member is obsolete
            {
#pragma warning disable CS0618 // Type or member is obsolete
                config.DisableServerCertificateValidation = true;
#pragma warning restore CS0618 // Type or member is obsolete
                await client.KV.Put(new KVPair("kv/reuseconfig") {Flags = 1000});
                Assert.Equal<ulong>(1000, (await client.KV.Get("kv/reuseconfig")).Response.Flags);
            }

#pragma warning disable CS0618 // Type or member is obsolete
            using (var client = new ConsulClient.ConsulClient(config))
#pragma warning restore CS0618 // Type or member is obsolete
            {
#pragma warning disable CS0618 // Type or member is obsolete
                config.DisableServerCertificateValidation = false;
#pragma warning restore CS0618 // Type or member is obsolete
                await client.KV.Put(new KVPair("kv/reuseconfig") {Flags = 2000});
                Assert.Equal<ulong>(2000, (await client.KV.Get("kv/reuseconfig")).Response.Flags);
            }

#pragma warning disable CS0618 // Type or member is obsolete
            using (var client = new ConsulClient.ConsulClient(config))
#pragma warning restore CS0618 // Type or member is obsolete
            {
                await client.KV.Delete("kv/reuseconfig");
            }
        }

        [Fact]
        public async Task Client_SetClientOptions()
        {
            var client = new ConsulClient.ConsulClient(c =>
            {
                c.Datacenter = "foo";
                c.WaitTime = new TimeSpan(0, 0, 100);
                c.Token = "12345";
            });
            var request = client.Get<KVPair>("/v1/kv/foo");

            await Assert.ThrowsAsync<ConsulRequestException>(async () => await request.Execute(CancellationToken.None));

            Assert.Equal("foo", request.Params["dc"]);
            Assert.Equal("1m40s", request.Params["wait"]);
        }

        [Fact]
        public async Task Client_SetQueryOptions()
        {
            var client = new ConsulClient.ConsulClient();
            var opts = new QueryOptions
            {
                Datacenter = "foo",
                Consistency = ConsistencyMode.Consistent,
                WaitIndex = 1000,
                WaitTime = new TimeSpan(0, 0, 100),
                Token = "12345"
            };
            var request = client.Get<KVPair>("/v1/kv/foo", opts);

            await Assert.ThrowsAsync<ConsulRequestException>(async () => await request.Execute(CancellationToken.None));

            Assert.Equal("foo", request.Params["dc"]);
            Assert.True(request.Params.ContainsKey("consistent"));
            Assert.Equal("1000", request.Params["index"]);
            Assert.Equal("1m40s", request.Params["wait"]);
        }

        [Fact]
        public async Task Client_SetWriteOptions()
        {
            var client = new ConsulClient.ConsulClient();

            var opts = new WriteOptions
            {
                Datacenter = "foo",
                Token = "12345"
            };

            var request = client.Put("/v1/kv/foo", new KVPair("kv/foo"), opts);

            await Assert.ThrowsAsync<ConsulRequestException>(async () => await request.Execute(CancellationToken.None));

            Assert.Equal("foo", request.Params["dc"]);
        }
    }
}