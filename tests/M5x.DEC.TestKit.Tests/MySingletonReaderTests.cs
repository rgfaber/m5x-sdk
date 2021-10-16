using System.Threading.Tasks;
using M5x.Config;
using M5x.DEC.TestKit.Integration.Qry;
using M5x.DEC.TestKit.Tests.SuT;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Tests
{
    public class MySingletonReaderTests : SingletonReaderTests<IMySingletonReader, MySingletonQuery, MyReadModel>
    {
        public MySingletonReaderTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        protected override void Initialize()
        {
            Query = MyTestContract.SingletonQuery;
            Reader = Container.GetRequiredService<IMySingletonReader>();
        }

        protected override void SetTestEnvironment()
        {
            DotEnv.FromEmbedded();
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
            services.AddMyFakeReaders();
        }
    }
}