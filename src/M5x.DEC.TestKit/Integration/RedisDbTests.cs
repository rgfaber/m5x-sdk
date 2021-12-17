using System.Threading.Tasks;
using M5x.Redis;
using M5x.Testing;
using StackExchange.Redis;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration;

public abstract class RedisDbTests<TReadModel> : ConnectionTests<IConnectionMultiplexer>
{
    public IRedisDb Database;
    protected object Factory;

    public RedisDbTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
    {
    }

    public object ReadModel { get; set; }

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
        Assert.Equal(expectedValue, value);
        return Task.CompletedTask;
    }

    [Fact]
    public Task Needs_Database()
    {
        Assert.NotNull(Database);
        return Task.CompletedTask;
    }

    [Fact]
    public Task Needs_ReadModel()
    {
        Assert.NotNull(ReadModel);
        return Task.CompletedTask;
    }
}