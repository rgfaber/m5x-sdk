using System;
using System.Security.Cryptography;
using System.Threading;

namespace M5x.Chassis.Mh.Support
{
    /// <summary> Provides statistically relevant random number generation </summary>
    public static class RRandom
    {
        private static readonly ThreadLocal<RandomNumberGenerator> Random =
            new(RandomNumberGenerator.Create);

        public static long NextLong()
        {
            var buffer = new byte[sizeof(long)];
            Random.Value.GetBytes(buffer);
            var value = BitConverter.ToInt64(buffer, 0);
            return value;
        }

        public static double NextDouble()
        {
            var l = NextLong();
            if (l == long.MinValue) l = 0;
            return (l + .0) / long.MaxValue;
        }
    }
}