using System.Collections.Generic;
using M5x.Chassis.Mh.Core;

namespace M5x.Chassis.Mh.Stats;

/// <summary>
///     A statistically representative sample of a data stream
/// </summary>
public interface ISample<out T> : ISample, ICopyable<T>
{
}

/// <summary>
///     A statistically representative sample of a data stream
/// </summary>
public interface ISample
{
    int Count { get; }
    ICollection<long> Values { get; }
    void Clear();
    void Update(long value);
}