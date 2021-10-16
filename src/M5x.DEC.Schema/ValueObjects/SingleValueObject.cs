using System;
using System.Collections.Generic;
using System.Reflection;
using M5x.DEC.Schema.Extensions;

namespace M5x.DEC.Schema.ValueObjects
{
    public interface ISingleValueObject
    {
        object GetValue();
    }

    public abstract record SingleValueObject<T> : ValueObject, IComparable, ISingleValueObject
        where T : IComparable
    {
        private static readonly Type Type = typeof(T);
        private static readonly TypeInfo TypeInfo = typeof(T).GetTypeInfo();

        protected SingleValueObject(T value)
        {
            if (TypeInfo.IsEnum && !Enum.IsDefined(Type, value))
                throw new ArgumentException($"The value '{value}' isn't defined in enum '{Type.PrettyPrint()}'");

            Value = value;
        }

        protected SingleValueObject()
        {
        }

        public T Value { get; }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) throw new ArgumentNullException(nameof(obj));

            var other = obj as SingleValueObject<T>;
            if (other == null)
                throw new ArgumentException(
                    $"Cannot compare '{GetType().PrettyPrint()}' and '{obj.GetType().PrettyPrint()}'");

            return Value.CompareTo(other.Value);
        }

        public object GetValue()
        {
            return Value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString()
        {
            return ReferenceEquals(Value, null)
                ? string.Empty
                : Value.ToString();
        }
    }
}