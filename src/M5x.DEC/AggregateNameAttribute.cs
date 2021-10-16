using System;

namespace M5x.DEC
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AggregateNameAttribute : Attribute
    {
        public AggregateNameAttribute(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }

        public string Name { get; }
    }
}