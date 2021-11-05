using M5x.Config;
using M5x.DEC.TestKit.Integration;
using M5x.DEC.TestKit.Tests.SuT;
using M5x.Redis;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Xunit.Abstractions;

namespace M5x.DEC.Infra.Tests.Redis
{
    public class MyRedisDbTests : RedisDbTests<MyReadModel>
    {
        public MyRedisDbTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        protected override void Initialize()
        {
            Database = Container.GetRequiredService<IRedisDb>();
            ReadModel = MyTestSchema.Model;
            Connection = Container.GetRequiredService<IConnectionMultiplexer>();
        }

        protected override void SetTestEnvironment()
        {
            DotEnv.FromEmbedded();
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
            services
                .AddSingletonRedisDb();
        }
    }
}