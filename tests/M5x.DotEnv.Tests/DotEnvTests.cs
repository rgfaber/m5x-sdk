using System;
using Xunit;

namespace M5x.DotEnv.Tests
{
    public class DotEnvTests
    {
        [Fact]
        public void Should_LoadFromEmbeddedResource()
        {
            Config.DotEnv.FromEmbedded();
            AssertEnv();
        }

        private void AssertEnv()
        {
            var name = Environment.GetEnvironmentVariable(EnVars.NAME);
            var address = Environment.GetEnvironmentVariable(EnVars.ADDRESS);
            Assert.Equal("Aleksander", name);
            Assert.Equal("Frankfurt", address);
        }


        [Fact]
        public void Should_Get()
        {
            Config.DotEnv.FromEmbedded();
            var name = Config.DotEnv.Get(EnVars.NAME);
            Assert.Equal("Aleksander", name);
        }
    }
}