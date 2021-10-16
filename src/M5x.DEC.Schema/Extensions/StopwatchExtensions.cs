using System;
using System.Diagnostics;
using System.Globalization;

namespace M5x.DEC.Schema.Extensions
{
    public static class StopwatchExtensions
    {
        /// <summary>
        ///     Return the interval as a string in the format 'n.nnn Seconds'.
        /// </summary>
        /// <returns>duration as a string</returns>
        public static string ToStringInSeconds(this Stopwatch stopwatch)
        {
            var elapsedSeconds = stopwatch.ElapsedMilliseconds / 1000.0;
            return $"{elapsedSeconds.ToString("N03", CultureInfo.CurrentCulture)} sec.";
        }

        /// <summary>
        ///     Return the interval as a string in the format 'm:n.nnn Seconds'.
        /// </summary>
        /// <returns>duration as a string</returns>
        public static string ToStringInMinutesAndSeconds(this Stopwatch stopwatch)
        {
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            var elapsedSeconds = elapsedMilliseconds == 0 ? 0 : (long)Math.Truncate(elapsedMilliseconds / 1000.0);
            var elapsedMinutes = Math.Abs(elapsedSeconds) < 1 ? 0 : (long)Math.Truncate(elapsedSeconds / 60.0);
            elapsedMilliseconds %= 1000;
            elapsedSeconds %= 60;
            return elapsedMinutes == 0
                ? $"{elapsedSeconds}.{elapsedMilliseconds:D3} sec."
                : $"{elapsedMinutes}:{elapsedSeconds:D2}.{elapsedMilliseconds:D3} min.";
        }
    }
}