using System.Globalization;
using System.Threading;

namespace M5x.Chassis.Mh.Support
{
    /// <summary>
    ///     Provides support for volatile operations around a <see cref="double" /> value
    ///     See: http://stackoverflow.com/questions/4727068/why-not-volatile-on-system-double-and-system-long
    /// </summary>
    public struct VolatileDouble
    {
        private double _value;

        public static VolatileDouble operator +(VolatileDouble left, VolatileDouble right)
        {
            return Add(left, right);
        }

        private static VolatileDouble Add(VolatileDouble left, VolatileDouble right)
        {
            left.Set(left.Get() + right.Get());
            return left.Get();
        }

        public static VolatileDouble operator -(VolatileDouble left, VolatileDouble right)
        {
            left.Set(left.Get() - right.Get());
            return left.Get();
        }

        public static VolatileDouble operator *(VolatileDouble left, VolatileDouble right)
        {
            left.Set(left.Get() * right.Get());
            return left.Get();
        }

        public static VolatileDouble operator /(VolatileDouble left, VolatileDouble right)
        {
            left.Set(left.Get() / right.Get());
            return left.Get();
        }

        private VolatileDouble(double value) : this()
        {
            Set(value);
        }

        public void Set(double value)
        {
            Volatile.Write(ref _value, value);
        }

        public double Get()
        {
            return Volatile.Read(ref _value);
        }

        public static implicit operator VolatileDouble(double value)
        {
            return new VolatileDouble(value);
        }

        public static implicit operator double(VolatileDouble value)
        {
            return value.Get();
        }

        public override string ToString()
        {
            return Get().ToString(CultureInfo.InvariantCulture);
        }
    }
}