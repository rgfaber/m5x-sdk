using M5x.Schemas;
using Xunit;

namespace M5x.DEC.Tests
{
    public class DomainTests
    {
        private const string Prefix = "test-";


        [Fact]
        public void Try_CreateAnId()
        {
            var testId = TestAggregateId.NewTestID();
            Assert.NotNull(testId);
            Assert.StartsWith(Prefix, testId.Value);
        }

        [IDPrefix(Prefix)]
        internal record TestAggregateId : AggregateId<TestAggregateId>
        {
            public TestAggregateId(string id) : base(id)
            {
            }

            public static TestAggregateId NewTestID()
            {
                return new(GuidFactories.NewCleanGuid);
            }
        }
    }
}