using System.Threading.Tasks;
using AutoBogus;
using Bogus;
using M5x.Config;
using M5x.DEC.TestKit.Integration.Qry;
using M5x.DEC.TestKit.Tests.SuT;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Tests
{
    
    
    
    public class MyPagedReaderTests: EnumerableReaderTests<IMyReader, MyPagedQry, MyReadModel>
    {

        
        
        public MyPagedReaderTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        protected override void Initialize()
        {
            Query = MyTestContract.PagedQry;
            Reader = Container.GetRequiredService<IMyReader>();;
        }

        protected override void SetTestEnvironment()
        {
            DotEnv.FromEmbedded();
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
            services.AddMyReader();
        }
    }
}