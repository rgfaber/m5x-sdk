using M5x.DEC.TestKit.Integration.Etl;
using M5x.DEC.TestKit.Tests.SuT;
using M5x.DEC.TestKit.Tests.SuT.Infra.Redis;
using M5x.Redis;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Tests
{
    public class MyRedisWriterTests :
    EventWriterTests<IMyRedisEventWriter, IMySingletonRedisReader, MyID, MyEvent, MyReadModel, MySingletonQuery>
    {
        public MyRedisWriterTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        protected override void Initialize()
        {
            Writer = Container.GetRequiredService<IMyRedisEventWriter>();
            Reader = Container.GetRequiredService<IMySingletonRedisReader>();
            Event = MyTestDomain.Event;
            Query = MyTestContract.SingletonQuery;
        }

        protected override void SetTestEnvironment()
        {
            
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
            services
                .AddSingletonRedisDb()
                .AddMyRedisWriter()
                .AddMyRedisReaders();
        }
    }
}