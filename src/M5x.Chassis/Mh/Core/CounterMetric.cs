using System.Runtime.Serialization;
using System.Text;
using M5x.Chassis.Mh.Support;

namespace M5x.Chassis.Mh.Core;

/// <summary>
///     An atomic counter metric
/// </summary>
public sealed class CounterMetric : IMetric
{
    private readonly AtomicLong _count = new(0);

    public CounterMetric()
    {
    }

    private CounterMetric(long count)
    {
        _count.Set(count);
    }

    public long Count => _count.Get();

    [IgnoreDataMember] public IMetric Copy => new CounterMetric(_count.Get());

    public void LogJson(StringBuilder sb)
    {
        sb.Append("{\"count\":").Append(Count).Append("}");
    }

    public void Increment()
    {
        Increment(1);
    }

    public void Increment(long amount)
    {
        _count.AddAndGet(amount);
    }

    public void Decrement()
    {
        Decrement(1);
    }

    public void Decrement(long amount)
    {
        _count.AddAndGet(0 - amount);
    }

    public void Clear()
    {
        _count.Set(0);
    }
}