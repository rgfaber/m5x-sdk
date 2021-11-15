using M5x.DEC.Schema;
using M5x.DEC.TestKit.Tests.SuT;
using M5x.DEC.TestKit.Unit.Contract;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Tests
{
    public class MyFactTests : FactTests<MyFact>
    {
        public MyFactTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        protected override void Initialize()
        {
            Fact = MyFact.New(AggregateInfo.New(MyTestSchema.TestID.Value),
                MyTestSchema.TEST_CORRELATION_ID,
                MyTestSchema.Payload);
        }

        protected override void SetTestEnvironment()
        {
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
        }

        protected override string GetExpectedFactTopic()
        {
            return MyConfig.Facts.MyFact;
        }
    }
}