using System;

namespace M5x.Chassis.Mh.Stats;

internal class DateTimeOffsetProvider : IDateTimeOffsetProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}