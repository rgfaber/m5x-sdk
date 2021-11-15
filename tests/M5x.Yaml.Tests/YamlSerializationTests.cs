using System;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Yaml.Tests
{
    public class YamlTests : IoCTestsBase
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private IYamlDeserializer _deserializer;
        private IYamlSerializer _serializer;


        public YamlTests(ITestOutputHelper output, IoCTestContainer container, ITestOutputHelper testOutputHelper) :
            base(output, container)
        {
            _testOutputHelper = testOutputHelper;
        }


        [Fact]
        public void Try_Serialization()
        {
            var test = TestHelper.Create<TestClassEnv>();
            foreach (var testTag in test.TestClass.Tags) testTag.Id = $"test:{testTag.Id}";
            var testSerialized = _serializer.Serialize(test);
            Assert.False(string.IsNullOrWhiteSpace(testSerialized));
            try
            {
                var other = _deserializer.Deserialize<TestClassEnv>(testSerialized);
                Assert.Null(other);
            }
            catch (Exception e)
            {
                _testOutputHelper.WriteLine(e.ToString());
                throw;
            }
        }

        protected override void Initialize()
        {
            _serializer = Container.GetService<IYamlSerializer>();
            _deserializer = Container.GetService<IYamlDeserializer>();
        }

        protected override void SetTestEnvironment()
        {
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
            services.AddTestClassGenerator();
        }
    }
}