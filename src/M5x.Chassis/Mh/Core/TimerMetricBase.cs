using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M5x.Chassis.Mh.Core;

public abstract class TimerMetricBase : IMetric, IMetered
{
    protected readonly HistogramMetric Histogram;
    protected readonly MeterMetric Meter;

    protected TimerMetricBase(TimeUnit durationUnit, TimeUnit rateUnit) : this(durationUnit, rateUnit,
        MeterMetric.New("calls", rateUnit), new HistogramMetric(HistogramMetric.SampleType.Biased), true)
    {
    }

    protected TimerMetricBase(TimeUnit durationUnit, TimeUnit rateUnit, MeterMetric meter,
        HistogramMetric histogram, bool clear)
    {
        DurationUnit = durationUnit;
        RateUnit = rateUnit;
        Meter = meter;
        Histogram = histogram;
        if (clear) Clear();
    }

    /// <summary>
    ///     Returns the timer's duration scale unit
    /// </summary>
    public TimeUnit DurationUnit { get; }

    /// <summary>
    ///     Returns the longest recorded duration
    /// </summary>
    public double Max => ConvertFromNanos(Histogram.Max);

    /// <summary>
    ///     Returns the shortest recorded duration
    /// </summary>
    public double Min => ConvertFromNanos(Histogram.Min);

    /// <summary>
    ///     Returns the arithmetic mean of all recorded durations
    /// </summary>
    public double Mean => ConvertFromNanos(Histogram.Mean);

    /// <summary>
    ///     Returns the standard deviation of all recorded durations
    /// </summary>
    public double StdDev => ConvertFromNanos(Histogram.StdDev);

    /// <summary>
    ///     Returns a list of all recorded durations in the timers's sample
    /// </summary>
    public ICollection<double> Values => Histogram.Values.Select(value => ConvertFromNanos(value)).ToList();

    /// <summary>
    ///     Returns the meter's rate unit
    /// </summary>
    /// <returns></returns>
    public TimeUnit RateUnit { get; }

    /// <summary>
    ///     Returns the number of events which have been marked
    /// </summary>
    /// <returns></returns>
    public long Count => Histogram.Count;

    /// <summary>
    ///     Returns the fifteen-minute exponentially-weighted moving average rate at
    ///     which events have occured since the meter was created
    ///     <remarks>
    ///         This rate has the same exponential decay factor as the fifteen-minute load
    ///         average in the top Unix command.
    ///     </remarks>
    /// </summary>
    public double FifteenMinuteRate => Meter.FifteenMinuteRate;

    /// <summary>
    ///     Returns the five-minute exponentially-weighted moving average rate at
    ///     which events have occured since the meter was created
    ///     <remarks>
    ///         This rate has the same exponential decay factor as the five-minute load
    ///         average in the top Unix command.
    ///     </remarks>
    /// </summary>
    public double FiveMinuteRate => Meter.FiveMinuteRate;

    /// <summary>
    ///     Returns the mean rate at which events have occured since the meter was created
    /// </summary>
    public double MeanRate => Meter.MeanRate;

    /// <summary>
    ///     Returns the one-minute exponentially-weighted moving average rate at
    ///     which events have occured since the meter was created
    ///     <remarks>
    ///         This rate has the same exponential decay factor as the one-minute load
    ///         average in the top Unix command.
    ///     </remarks>
    /// </summary>
    /// <returns></returns>
    public double OneMinuteRate => Meter.OneMinuteRate;

    /// <summary>
    ///     Returns the type of events the meter is measuring
    /// </summary>
    /// <returns></returns>
    public string EventType => Meter.EventType;

    public void LogJson(StringBuilder sb)
    {
        var percSb = new StringBuilder();
        var percentiles = Percentiles(0.5, 0.75, 0.95, 0.98, 0.99, 0.999);
        percSb.Append("{\"0.5\":").Append(percentiles[0]);
        percSb.Append(",\"0.75\":").Append(percentiles[1]);
        percSb.Append(",\"0.95\":").Append(percentiles[2]);
        percSb.Append(",\"0.98\":").Append(percentiles[3]);
        percSb.Append(",\"0.99\":").Append(percentiles[4]);
        percSb.Append(",\"0.999\":").Append(percentiles[5]);
        percSb.Append("}");

        sb.Append("{\"count\":").Append(Count)
            .Append(",\"duration unit\":\"").Append(DurationUnit).Append("\"")
            .Append(",\"rate unit\":\"").Append(RateUnit).Append("\"")
            .Append(",\"fifteen minute rate\":").Append(FifteenMinuteRate)
            .Append(",\"five minute rate\":").Append(FiveMinuteRate)
            .Append(",\"one minute rate\":").Append(OneMinuteRate)
            .Append(",\"mean rate\":").Append(MeanRate)
            .Append(",\"max\":").Append(Max)
            .Append(",\"min\":").Append(Min)
            .Append(",\"mean\":").Append(Mean)
            .Append(",\"stdev\":").Append(StdDev)
            .Append(",\"percentiles\":").Append(percSb).Append("}");
    }

    public abstract IMetric Copy { get; }

    /// <summary>
    ///     Clears all recorded durations
    /// </summary>
    private void Clear()
    {
        Histogram.Clear();
    }

    protected void Update(long duration, TimeUnit unit)
    {
        Update(unit.ToNanos(duration));
    }

    /// <summary>
    ///     Returns an array of durations at the given percentiles
    /// </summary>
    public double[] Percentiles(params double[] percentiles)
    {
        var scores = Histogram.Percentiles(percentiles);
        for (var i = 0; i < scores.Length; i++) scores[i] = ConvertFromNanos(scores[i]);

        return scores;
    }

    protected void Update(long duration)
    {
        if (duration < 0) return;
        Histogram.Update(duration);
        Meter.Mark();
    }

    private double ConvertFromNanos(double nanos)
    {
        return nanos / DurationUnit.Convert(1, TimeUnit.Nanoseconds);
    }
}