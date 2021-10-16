using System;
using System.Threading.Tasks;
using M5x.Consul.Client;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Consul.Tests
{
    public class OperatorTest : ConsulTestsBase, IDisposable
    {
        public OperatorTest(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }


        //[Fact]
        //public async Task Operator_KeyringInstallListPutRemove()
        //{
        //    const string oldKey = "d8wu8CSUrqgtjVsvcBPmhQ==";
        //    const string newKey = "qxycTi/SsePj/TZzCBmNXw==";

        //    using (var c = new ConsulClient.ConsulClient())
        //    {
        //        await c.Operator.KeyringInstall(oldKey);
        //        await c.Operator.KeyringUse(oldKey);
        //        await c.Operator.KeyringInstall(newKey);

        //        var listResponses = await c.Operator.KeyringList();

        //        Assert.Equal(2, listResponses.Response.Length);

        //        foreach (var response in listResponses.Response)
        //        {
        //            Assert.Equal(2, response.Keys.Count);
        //            Assert.True(response.Keys.ContainsKey(oldKey));
        //            Assert.True(response.Keys.ContainsKey(newKey));
        //        }

        //        await c.Operator.KeyringUse(newKey);

        //        await c.Operator.KeyringRemove(oldKey);

        //        listResponses = await c.Operator.KeyringList();
        //        Assert.Equal(2, listResponses.Response.Length);

        //        foreach (var response in listResponses.Response)
        //        {
        //            Assert.Equal(1, response.Keys.Count);
        //            Assert.False(response.Keys.ContainsKey(oldKey));
        //            Assert.True(response.Keys.ContainsKey(newKey));
        //        }
        //    }
        //}

        [Fact]
        public async Task Operator_RaftGetConfiguration()
        {
            using (var client = new ConsulClient.ConsulClient())
            {
                var servers = await client.Operator.RaftGetConfiguration();

                Assert.True(servers.Response.Servers.Count == 1);
                Assert.True(servers.Response.Servers[0].Leader);
                Assert.True(servers.Response.Servers[0].Voter);
            }
        }

        [Fact]
        public async Task Operator_RaftRemovePeerByAddress()
        {
            using (var client = new ConsulClient.ConsulClient())
            {
                try
                {
                    await client.Operator.RaftRemovePeerByAddress("nope");
                }
                catch (ConsulRequestException e)
                {
                    Assert.Contains("address \"nope\" was not found in the Raft configuration", e.Message);
                }
            }
        }
    }
}