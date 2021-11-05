using System.Threading.Tasks;
using M5x.DEC.Infra;
using M5x.DEC.TestKit.Integration.Etl;
using M5x.DEC.TestKit.Tests.SuT;
using M5x.DEC.TestKit.Tests.SuT.Infra.Redis;
using M5x.Redis;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Tests
{
    public class MyRedisWriterTests :
    EventWriterTests<IMyRedisEventWriter, IMyInterpreter, IMySingletonRedisReader, MyEvent, MyReadModel,
        MySingletonQuery>
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
            Interpreter = Container.GetRequiredService<IInterpreter<MyReadModel, MyEvent>>();
        }

        protected override void SetTestEnvironment()
        {
            
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
            services
                .AddMyInterpreter()
                .AddSingletonRedisDb()
                .AddMyRedisWriter()
                .AddMyRedisReaders();
        }

        [Fact]
        public async Task Should_BeAbleToWrite1000Events()
        {
            using var faker = MyBogus.Domain.UniqueCommand.Generate(1000).GetEnumerator();
            do
            {
                var cmd = faker.Current;
                if(cmd==null) continue;
                var evt = MyEvent.New(cmd);
                await Writer.HandleAsync(evt);
                var exp = await Reader.GetByIdAsync(@evt.Meta.Id);
                exp.Content.ShouldBeEquivalentTo(evt.Payload);
            } while (faker.MoveNext());
        }
        
        
        [Fact]
        public async Task Should_BeAbleToWrite10000Events()
        {
            using var faker = MyBogus.Domain.UniqueCommand.Generate(10000).GetEnumerator();
            do
            {
                var cmd = faker.Current;
                if(cmd==null) continue;
                var evt = MyEvent.New(cmd);
                await Writer.HandleAsync(evt);
                var exp = await Reader.GetByIdAsync(@evt.Meta.Id);
                exp.Content.ShouldBeEquivalentTo(evt.Payload);
            } while (faker.MoveNext());
        }

        
        
        
        
    }
}