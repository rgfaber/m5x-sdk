using System;
using System.Globalization;

namespace M5x.Utils;

public static class DateTimeUtils
{
    /// <summary>
    /// </summary>
    public const string DateFormat = "dd/MM/yyyy";


    /// <summary>
    ///     An offset to the UTC DateTime giving the current Date and Time
    /// </summary>
    public static TimeSpan UtcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);


    /// <summary>
    ///     Gets the current UTC time.
    /// </summary>
    /// <value>The UTC time.</value>
    public static DateTime UtcTime => DateTime.Now - UtcOffset;

    /// <summary>
    ///     Gets the current UTC Date.
    /// </summary>
    /// <value>The UTC today.</value>
    public static DateTime UtcToday => UtcTime.Date;

    /// <summary>
    ///     Parses the date.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns></returns>
    public static DateTime? ParseDate(string date)
    {
        return DateTime.ParseExact(date, DateFormat, CultureInfo.InvariantCulture);
    }
}