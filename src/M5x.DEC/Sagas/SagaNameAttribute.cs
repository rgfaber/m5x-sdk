using System;

namespace M5x.DEC.Sagas
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SagaNameAttribute : Attribute
    {
        public SagaNameAttribute(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }

        public string Name { get; }
    }
}