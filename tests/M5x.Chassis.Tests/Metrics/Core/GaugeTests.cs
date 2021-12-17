using System.Collections.Generic;
using M5x.Chassis.Mh.Core;
using Xunit;

namespace M5x.Chassis.Tests.Metrics.Core;

public class GaugeTests : IClassFixture<MetricsFixture>
{
    private readonly MetricsFixture _fixture;

    public GaugeTests(MetricsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Can_gauge_scalar_value()
    {
        var queue = new Queue<int>();
        var gauge = new GaugeMetric<int>(() => queue.Count);

        queue.Enqueue(5);
        Assert.Equal(1, gauge.Value);

        queue.Enqueue(6);
        queue.Dequeue();
        Assert.Equal(1, gauge.Value);

        queue.Dequeue();
        Assert.Equal(0, gauge.Value);
    }

    [Fact]
    public void Can_use_gauge_metric()
    {
        var queue = new Queue<int>();
        var gauge = _fixture.M.Gauge(typeof(GaugeTests), "Can_use_gauge_metric", () => queue.Count);
        queue.Enqueue(5);
        Assert.Equal(1, gauge.Value);
    }
}