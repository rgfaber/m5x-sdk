namespace M5x.Yaml.Tests
{
    public class TestClassYamlGenerator : ITestClassGenerator
    {
        private readonly IYamlSerializer _serializer;

        public TestClassYamlGenerator(IYamlSerializer serializer)
        {
            _serializer = serializer;
        }

        public string Generate(TestClass test)
        {
            var res = _serializer.Serialize(test);
            return res;
        }
    }


    public interface ITestClassGenerator
    {
        string Generate(TestClass test);
    }
}