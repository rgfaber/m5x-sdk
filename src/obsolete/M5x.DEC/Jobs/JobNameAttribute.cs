using System;

namespace M5x.DEC.Jobs
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class JobNameAttribute : Attribute
    {
        public JobNameAttribute(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }

        public string Name { get; }
    }
}