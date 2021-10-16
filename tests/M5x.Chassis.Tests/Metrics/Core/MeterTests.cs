using System.Threading;
using M5x.Chassis.Mh;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Chassis.Tests.Metrics.Core
{
    public class MeterTests : IClassFixture<MetricsFixture>
    {
        private readonly MetricsFixture _fixture;
        private readonly ITestOutputHelper _testOutputHelper;

        public MeterTests(MetricsFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture;
            _testOutputHelper = testOutputHelper;
        }


        [Fact]
        public void Can_count()
        {
            var meter = _fixture.M.Meter(typeof(MeterTests), "Can_count", "test", TimeUnit.Seconds);
            meter.Mark(3);
            Assert.Equal(3, meter.Count);
        }

        [Fact]
        public void Can_meter()
        {
            const int count = 100000;
            var block = new ManualResetEvent(false);
            var meter = _fixture.M.Meter(typeof(MeterTests), "Can_meter", "test", TimeUnit.Seconds);
            Assert.NotNull(meter);

            var i = 0;
            ThreadPool.QueueUserWorkItem(s =>
            {
                while (i < count)
                {
                    meter.Mark();
                    i++;
                }

                Thread.Sleep(10000); // Wait for at least one EWMA rate tick
                block.Set();
            });
            block.WaitOne();

            Assert.Equal(count, meter.Count);

            var oneMinuteRate = meter.OneMinuteRate;
            var fiveMinuteRate = meter.FiveMinuteRate;
            var fifteenMinuteRate = meter.FifteenMinuteRate;
            var meanRate = meter.MeanRate;

            Assert.True(oneMinuteRate > 0);

            //Console.WriteLine("One minute rate:" + meter.OneMinuteRate);

            Assert.True(fiveMinuteRate > 0);
            _testOutputHelper.WriteLine("Five minute rate:" + meter.FiveMinuteRate);

            Assert.True(fifteenMinuteRate > 0);
            _testOutputHelper.WriteLine("Fifteen minute rate:" + meter.FifteenMinuteRate);

            Assert.True(meanRate > 0);
        }
    }
}