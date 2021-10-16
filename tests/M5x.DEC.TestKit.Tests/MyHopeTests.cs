using System.Threading.Tasks;
using Castle.Core.Configuration;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Common;
using M5x.DEC.TestKit.Tests.SuT;
using M5x.DEC.TestKit.Unit.Contract;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Tests
{
    public class MyHopeTests: HopeTests<MyHope, MyFeedback>
    {
        public MyHopeTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        protected override void Initialize()
        {
            Hope = MyHope.New(MyTestSchema.TestID.Value, MyTestSchema.TEST_CORRELATION_ID,
                MyTestSchema.Payload);
            Feedback = MyFeedback.New(AggregateInfo.New(MyTestSchema.TestID.Value, -1,0), MyTestSchema.TEST_CORRELATION_ID, Dummy.Empty);
        }

        protected override void SetTestEnvironment()
        {
            
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
            
        }

        protected override string GetExpectedHopeTopic()
        {
            return MyConfig.Hopes.MyHope;
        }
    }
}