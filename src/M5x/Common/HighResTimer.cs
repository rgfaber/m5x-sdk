// ***********************************************************************
// <copyright file="HighResTimer.cs" company="Flint Group">
//     Copyright (c) Flint Group. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace M5x.Common
{
    /// <summary>
    ///     Provides functions for high precision time measurement
    /// </summary>
    public class HighResTimer
    {
        /// <summary>
        ///     The _frequency
        /// </summary>
        private long _frequency;

        /// <summary>
        ///     The _start
        /// </summary>
        private long _start;

        /// <summary>
        ///     The _stop
        /// </summary>
        private long _stop;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <exception cref="Win32Exception"></exception>
        /// <exception cref="System.ComponentModel.Win32Exception"></exception>
        public HighResTimer()
        {
            _start = 0;
            _stop = 0;
            _frequency = 0;
            if (QueryPerformanceFrequency(out _frequency) == false) throw new Win32Exception(); // timer not supported
        }

        /// <summary>
        ///     Return the duration of the measured interval in seconds
        /// </summary>
        /// <value>The duration.</value>
        public double Duration => (_stop - _start) / (double)_frequency;

        /// <summary>
        ///     Frequency of timer (number of counts in one second on this machine)
        /// </summary>
        /// <value>The frequency.</value>
        public long Frequency
        {
            get
            {
                QueryPerformanceFrequency(out _frequency);
                return _frequency;
            }
        }

        /// <summary>
        ///     Queries the performance counter.
        /// </summary>
        /// <param name="lpPerformanceCount">The lp performance count.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        /// <summary>
        ///     Queries the performance frequency.
        /// </summary>
        /// <param name="lpFrequency">The lp frequency.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        /// <summary>
        ///     Start the timer
        /// </summary>
        /// <returns>long - tick count</returns>
        public long Start()
        {
            QueryPerformanceCounter(out _start);
            return _start;
        }

        /// <summary>
        ///     Stop timer
        /// </summary>
        /// <returns>long - tick count</returns>
        public long Stop()
        {
            QueryPerformanceCounter(out _stop);
            return _stop;
        }

        /// <summary>
        ///     Return the duration as a string in the format 'n.nnn Seconds'.
        /// </summary>
        /// <returns>duration as a string</returns>
        public override string ToString()
        {
            return $"{Duration.ToString("N03", CultureInfo.CurrentCulture)} sec.";
        }
    }
}