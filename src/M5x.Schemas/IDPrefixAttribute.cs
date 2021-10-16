using System;

namespace M5x.Schemas
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IDPrefixAttribute : Attribute
    {
        public IDPrefixAttribute(string prefix)
        {
            Prefix = prefix;
        }

        public string Prefix { get; set; }
    }
}