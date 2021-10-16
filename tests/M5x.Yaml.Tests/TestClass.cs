using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace M5x.Yaml.Tests
{
    public class TestClass
    {
        public TestClass()
        {
            Tags = new List<Tag>();
            Details = new Details();
        }

        [YamlMember] public IEnumerable<Tag> Tags { get; set; }

        [YamlMember] public Details Details { get; set; }

        [YamlMember] public string Id { get; set; }
    }

    public class Details
    {
    }

    public class Tag
    {
        public string Id { get; set; }
    }
}