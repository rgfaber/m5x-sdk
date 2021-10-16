using System.Threading.Tasks;
using M5x.Config;
using M5x.DEC.Schema.Utils;
using M5x.DEC.TestKit.Integration.Etl;
using M5x.DEC.TestKit.Tests.SuT;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;


namespace M5x.DEC.TestKit.Tests
{
    public class MyWriterTests: WriterTests<IMyWriter, IMyReader, MyID, MyFact, MyReadModel,MyPagedQry>
    {
        public MyWriterTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        protected override void Initialize()
        {
            Fact = MyTestContract.Fact;
            Query = MyTestContract.PagedQry;
            Reader = Container.GetRequiredService<IMyReader>();
            Writer = Container.GetRequiredService<IMyWriter>();
        }

        protected override void SetTestEnvironment()
        {
            DotEnv.FromEmbedded();
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
            services
                .AddMyReader()
                .AddMyWriter();
        }

    }
}
