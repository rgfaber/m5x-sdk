using System;

namespace M5x.CEQS.Schema
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