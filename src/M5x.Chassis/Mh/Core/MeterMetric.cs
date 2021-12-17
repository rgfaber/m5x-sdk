using System;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using M5x.Chassis.Mh.Stats;
using M5x.Chassis.Mh.Support;

namespace M5x.Chassis.Mh.Core;

/// <summary>
///     A meter metric which measures mean throughput and one-, five-, and fifteen-minute exponentially-weighted moving
///     average throughputs.
/// </summary>
/// <see href="http://en.wikipedia.org/wiki/Moving_average#Exponential_moving_average">EMA</see>
public class MeterMetric : IMetric, IMetered, IDisposable
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(5);
    private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;

    private readonly CancellationTokenSource _token = new();
    private AtomicLong _count = new();
    private Ewma _m15Rate = Ewma.FifteenMinuteEwma();

    private Ewma _m1Rate = Ewma.OneMinuteEwma();
    private Ewma _m5Rate = Ewma.FiveMinuteEwma();
    private long _startTime = DateTime.UtcNow.Ticks;

    private MeterMetric(string eventType, TimeUnit rateUnit, IDateTimeOffsetProvider dateTimeOffsetProvider)
    {
        _dateTimeOffsetProvider = dateTimeOffsetProvider ?? new DateTimeOffsetProvider();

        EventType = eventType;
        RateUnit = rateUnit;
    }

    public void Dispose()
    {
        _token.Cancel();
    }

    /// <summary> Returns the meter's rate unit </summary>
    public TimeUnit RateUnit { get; }

    /// <summary> Returns the type of events the meter is measuring </summary>
    public string EventType { get; }

    /// <summary>
    ///     Returns the number of events which have been marked
    /// </summary>
    /// <returns></returns>
    public long Count => _count.Get();

    /// <summary>
    ///     Returns the fifteen-minute exponentially-weighted moving average rate at
    ///     which events have occured since the meter was created
    ///     <remarks>
    ///         This rate has the same exponential decay factor as the fifteen-minute load
    ///         average in the top Unix command.
    ///     </remarks>
    /// </summary>
    public double FifteenMinuteRate => _m15Rate.Rate(RateUnit);

    /// <summary>
    ///     Returns the five-minute exponentially-weighted moving average rate at
    ///     which events have occured since the meter was created
    ///     <remarks>
    ///         This rate has the same exponential decay factor as the five-minute load
    ///         average in the top Unix command.
    ///     </remarks>
    /// </summary>
    public double FiveMinuteRate => _m5Rate.Rate(RateUnit);

    /// <summary>
    ///     Returns the mean rate at which events have occured since the meter was created
    /// </summary>
    public double MeanRate
    {
        get
        {
            if (Count == 0)
                return 0.0;
            var elapsed =
                (_dateTimeOffsetProvider.UtcNow.Ticks - _startTime) * 100; // 1 DateTimeOffset Tick == 100ns
            return ConvertNanosRate(Count / (double)elapsed);
        }
    }

    /// <summary>
    ///     Returns the one-minute exponentially-weighted moving average rate at
    ///     which events have occured since the meter was created
    ///     <remarks>
    ///         This rate has the same exponential decay factor as the one-minute load
    ///         average in the top Unix command.
    ///     </remarks>
    /// </summary>
    /// <returns></returns>
    public double OneMinuteRate => _m1Rate.Rate(RateUnit);

    public void LogJson(StringBuilder sb)
    {
        sb.Append("{\"count\":").Append(Count)
            .Append(",\"rate unit\":\"").Append(RateUnit).Append("\"")
            .Append(",\"fifteen minute rate\":").Append(FifteenMinuteRate)
            .Append(",\"five minute rate\":").Append(FiveMinuteRate)
            .Append(",\"one minute rate\":").Append(OneMinuteRate)
            .Append(",\"mean rate\":").Append(MeanRate).Append("}");
    }

    [IgnoreDataMember]
    public IMetric Copy
    {
        get
        {
            var metric = new MeterMetric(EventType, RateUnit, _dateTimeOffsetProvider)
            {
                _startTime = _startTime,
                _count = Count,
                _m1Rate = _m1Rate,
                _m5Rate = _m5Rate,
                _m15Rate = _m15Rate
            };
            return metric;
        }
    }

    public static MeterMetric New(string eventType, TimeUnit rateUnit,
        IDateTimeOffsetProvider dateTimeOffsetProvider = null)
    {
        var meter = new MeterMetric(eventType, rateUnit, dateTimeOffsetProvider);

        Task.Run(async () =>
        {
            while (!meter._token.IsCancellationRequested)
            {
                await Task.Delay(Interval, meter._token.Token);
                meter.Tick();
            }
        }, meter._token.Token);

        return meter;
    }

    private void Tick()
    {
        _m1Rate.Tick();
        _m5Rate.Tick();
        _m15Rate.Tick();
    }

    /// <summary>
    ///     Mark the occurrence of a given number of events
    /// </summary>
    public void Mark(long n = 1)
    {
        _count.AddAndGet(n);
        _m1Rate.Update(n);
        _m5Rate.Update(n);
        _m15Rate.Update(n);
    }

    private double ConvertNanosRate(double ratePerNs)
    {
        return ratePerNs * RateUnit.ToNanos(1);
    }
}