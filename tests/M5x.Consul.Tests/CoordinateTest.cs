using System;
using System.Threading.Tasks;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Consul.Tests
{
    public class CoordinateTest : ConsulTestsBase, IDisposable
    {
        public CoordinateTest(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        [Fact]
        public async Task Coordinate_Datacenters()
        {
            var client = new ConsulClient.ConsulClient();

            var info = await client.Agent.Self();

            var datacenters = await client.Coordinate.Datacenters();

            Assert.NotNull(datacenters.Response);
            Assert.True(datacenters.Response.Length > 0);
        }

        [Fact]
        public async Task Coordinate_Nodes()
        {
            var client = new ConsulClient.ConsulClient();

            var info = await client.Agent.Self();

            var nodes = await client.Coordinate.Nodes();

            // There's not a good way to populate coordinates without
            // waiting for them to calculate and update, so the best
            // we can do is call the endpoint and make sure we don't
            // get an error. - from offical API.
            Assert.NotNull(nodes);
        }
    }
}