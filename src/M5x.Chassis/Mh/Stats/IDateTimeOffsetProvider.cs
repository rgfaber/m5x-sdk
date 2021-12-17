using System;

namespace M5x.Chassis.Mh.Stats;

public interface IDateTimeOffsetProvider
{
    DateTimeOffset UtcNow { get; }
}