using Xunit;

namespace M5x.Chassis.Tests.Metrics.Core;

public class CounterTests : IClassFixture<MetricsFixture>
{
    private readonly MetricsFixture _fixture;

    public CounterTests(MetricsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Can_count()
    {
        var counter = _fixture.M.Counter(typeof(CounterTests), "Can_count");
        Assert.NotNull(counter);

        counter.Increment(100);
        Assert.Equal(100, counter.Count);
    }
}