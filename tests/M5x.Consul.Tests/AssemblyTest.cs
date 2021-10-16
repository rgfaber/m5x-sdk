using System.Reflection;
using Xunit;

namespace M5x.Consul.Tests
{
    public class AssemblyTest
    {
        [Fact]
        public void Assembly_IsStrongNamed()
        {
            var type = typeof(ConsulClient.ConsulClient);
            var typeInfo = type.GetTypeInfo();
            var name = typeInfo.Assembly.FullName;
            Assert.Contains("PublicKeyToken", typeInfo.Assembly.FullName);
        }
    }
}