using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace M5x.DEC.Schema.ValueObjects
{
    public abstract record ValueObject
    {
        private static readonly ConcurrentDictionary<Type, IReadOnlyCollection<PropertyInfo>> TypeProperties =
            new();

        public override int GetHashCode()
        {
            unchecked
            {
                return GetEqualityComponents()
                    .Aggregate(17, (current, obj) => current * 23 + (obj?.GetHashCode() ?? 0));
            }
        }

        public override string ToString()
        {
            return $"{{{string.Join(", ", GetProperties().Select(f => $"{f.Name}: {f.GetValue(this)}"))}}}";
        }

        protected virtual IEnumerable<object> GetEqualityComponents()
        {
            return GetProperties().Select(x => x.GetValue(this));
        }

        protected virtual IEnumerable<PropertyInfo> GetProperties()
        {
            return TypeProperties.GetOrAdd(
                GetType(),
                t => t
                    .GetTypeInfo()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .OrderBy(p => p.Name)
                    .ToList());
        }
    }
}