using M5x.Schemas;

namespace M5x.DEC.Infra.Tests
{
    [IDPrefix("test")]
    public record TestAggregateId : AggregateId<TestAggregateId>
    {
        public TestAggregateId(string value) : base(value)
        {
        }

        
    }
}