using System.Threading.Tasks;
using M5x.Config;
using M5x.DEC.TestKit.Tests.SuT;
using M5x.DEC.TestKit.Unit.Contract;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Tests
{
    public class MyQueryTests: QueryTests<MyPagedQry, MyPagedResponse>
    {
        public MyQueryTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        protected override void Initialize()
        {
            Qry = MyPagedQry.New(MyTestSchema.TEST_CORRELATION_ID,1,5);
            Rsp = MyPagedResponse.New(
                MyTestSchema.TEST_CORRELATION_ID, 
                1, 
                MyBogus.Schema.ReadModel.Generate(5));

        }

        protected override void SetTestEnvironment()
        {
            DotEnv.FromEmbedded();
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
            
        }
    }
}