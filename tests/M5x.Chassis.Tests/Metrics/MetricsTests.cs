using Xunit;

namespace M5x.Chassis.Tests.Metrics
{
    public class MetricsTests : IClassFixture<MetricsFixture>
    {
        private readonly MetricsFixture _fixture;

        public MetricsTests(MetricsFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void Can_get_same_metric_when_metric_exists()
        {
            var counter = _fixture.M.Counter(typeof(MetricsTests), "Can_get_same_metric_when_metric_exists");
            Assert.NotNull(counter);
            var same = _fixture.M.Counter(typeof(MetricsTests), "Can_get_same_metric_when_metric_exists");
            Assert.Same(counter, same);
        }
    }
}