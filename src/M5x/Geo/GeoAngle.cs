// ***********************************************************************
// <copyright file="GeoAngle.cs" company="Flint Group">
//     Copyright (c) Flint Group. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;

namespace M5x.Geo
{
    /// <summary>
    ///     Class GeoAngle.
    /// </summary>
    public class GeoAngle
    {
        /// <summary>
        ///     Gets or sets a value indicating whether this instance is negative.
        /// </summary>
        /// <value><c>true</c> if this instance is negative; otherwise, <c>false</c>.</value>
        public bool IsNegative { get; set; }

        /// <summary>
        ///     Gets or sets the degrees.
        /// </summary>
        /// <value>The degrees.</value>
        public int Degrees { get; set; }

        /// <summary>
        ///     Gets or sets the minutes.
        /// </summary>
        /// <value>The minutes.</value>
        public int Minutes { get; set; }

        /// <summary>
        ///     Gets or sets the seconds.
        /// </summary>
        /// <value>The seconds.</value>
        public int Seconds { get; set; }

        /// <summary>
        ///     Gets or sets the milliseconds.
        /// </summary>
        /// <value>The milliseconds.</value>
        public int Milliseconds { get; set; }


        /// <summary>
        ///     Froms the double.
        /// </summary>
        /// <param name="angleInDegrees">The angle in degrees.</param>
        /// <returns>GeoAngle.</returns>
        public static GeoAngle FromDouble(double angleInDegrees)
        {
            //ensure the value will fall within the primary range [-180.0..+180.0]
            while (angleInDegrees < -180.0)
                angleInDegrees += 360.0;

            while (angleInDegrees > 180.0)
                angleInDegrees -= 360.0;

            var result = new GeoAngle();

            //switch the value to positive
            result.IsNegative = angleInDegrees < 0;
            angleInDegrees = Math.Abs(angleInDegrees);

            //gets the degree
            result.Degrees = (int)Math.Floor(angleInDegrees);
            var delta = angleInDegrees - result.Degrees;

            //gets minutes and seconds
            var seconds = (int)Math.Floor(3600.0 * delta);
            result.Seconds = seconds % 60;
            result.Minutes = (int)Math.Floor(seconds / 60.0);
            delta = delta * 3600.0 - seconds;

            //gets fractions
            result.Milliseconds = (int)(1000.0 * delta);

            return result;
        }


        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            var degrees = IsNegative
                ? -Degrees
                : Degrees;

            return $"{degrees:000}°{Minutes:00}'{Seconds:00}\"";
        }


        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public string ToString(string format)
        {
            switch (format)
            {
                case "NS":
                    return $"{Degrees:000}°{Minutes:00}'{Seconds:00}\".{Milliseconds:000} {(IsNegative ? 'S' : 'N')}";

                case "WE":
                    return $"{Degrees:000}°{Minutes:00}'{Seconds:00}\".{Milliseconds:000} {(IsNegative ? 'W' : 'E')}";

                default:
                    throw new NotImplementedException();
            }
        }
    }
}