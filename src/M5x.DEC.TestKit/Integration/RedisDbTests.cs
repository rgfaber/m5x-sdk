using System.Threading.Tasks;
using M5x.Redis;
using M5x.Testing;
using StackExchange.Redis;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration
{
    public abstract class RedisDbTests<TReadModel> : ConnectionTests<IConnectionMultiplexer>
    {
        protected object Factory;
        
        public IRedisDb Database;
        
        public object ReadModel { get; set; }

        public RedisDbTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        [Fact]
        public Task Must_ReadModelMustBeAssignableFromTReadModel()
        {
            Assert.IsAssignableFrom<TReadModel>(ReadModel);
            return Task.CompletedTask;
        }


        [Fact]
        public Task Must_BeAbleToReadWriteRedisStrings()
        {
            var key = new RedisKey("MyTest");
            var expectedValue = "My TestValue";
            Database.DB.StringSetAsync(key, expectedValue);
            var value = Database.DB.StringGetAsync(key).Result;
            Assert.Equal(expectedValue, (string)value);
            return Task.CompletedTask;
        }

        [Fact]
        public void Needs_Database()
        {
            Assert.NotNull(Database);
        }

        [Fact]
        public void Needs_ReadModel()
        {
            Assert.NotNull(ReadModel);
        }
    }
}