using System;

namespace M5x.Chassis.Mh.Core
{
    /// <summary>
    ///     A hash key for storing metrics associated by the parent class and name pair
    /// </summary>
    public struct MetricName : IComparable<MetricName>
    {
        public string Name { get; }

        public string Context { get; }

        public MetricName(string context, string name)
        {
            Name = name;
            Context = context;
        }

        public MetricName(Type @class, string name)
            : this()
        {
            if (@class == null) throw new ArgumentNullException(nameof(@class));
            if (name == null) throw new ArgumentNullException(nameof(name));
            Context = @class.FullName;
            Name = name;
        }

        public bool Equals(MetricName other)
        {
            return string.Equals(Name, other.Name) && string.Equals(Context, other.Context);
        }

        public int CompareTo(MetricName other)
        {
            var r = string.Compare(Context, other.Context, StringComparison.OrdinalIgnoreCase);
            if (r != 0)
                return r;
            return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public static bool operator ==(MetricName x, MetricName y)
        {
            return x.CompareTo(y) == 0;
        }

        public static bool operator !=(MetricName x, MetricName y)
        {
            return !(x == y);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is MetricName && Equals((MetricName)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name?.GetHashCode() ?? 0) * 397) ^ (Context?.GetHashCode() ?? 0);
            }
        }
    }
}