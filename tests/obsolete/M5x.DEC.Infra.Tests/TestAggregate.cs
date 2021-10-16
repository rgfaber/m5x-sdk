namespace M5x.DEC.Infra.Tests
{
    public class TestAggregate: AggregateRoot<TestAggregateId>
    {
        public TestAggregate(TestAggregateId id) : base(id)
        {
        }
    }
}