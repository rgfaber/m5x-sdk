using YamlDotNet.Serialization;

namespace M5x.Yaml.Tests
{
    public class TestClassEnv
    {
        public TestClassEnv()
        {
            TestClass = new TestClass();
        }

        [YamlMember] public TestClass TestClass { get; set; }
    }
}