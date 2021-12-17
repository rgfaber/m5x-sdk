using System;
using System.Threading;

namespace M5x.Chassis.Mh.Support;

internal class ThreadLocalRandom
{
    private static readonly Random Seeder = new();
    private static readonly ThreadLocal<int> Seed;

    private static ThreadLocal<Random> _random;

    static ThreadLocalRandom()
    {
        lock (Seeder)
        {
            Seed = new ThreadLocal<int>(() => Seeder.Next());
        }
    }

    public static double NextNonZeroDouble()
    {
        EnsureInitialized();
        var r = _random.Value.NextDouble();
        return Math.Max(r, double.Epsilon);
    }

    private static void EnsureInitialized()
    {
        if (_random == null) _random = new ThreadLocal<Random>(() => new Random(Seed.Value));
    }
}