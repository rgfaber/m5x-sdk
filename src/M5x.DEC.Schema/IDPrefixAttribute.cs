using System;

namespace M5x.DEC.Schema
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


    public static class Helper
    {
        public static string GetIdPrefix<TAggregateId>(this TAggregateId aggregateId) 
            where TAggregateId:IIdentity
        {
            var prefixAttributes =
                (IDPrefixAttribute[])typeof(TAggregateId).GetCustomAttributes(typeof(IDPrefixAttribute), true);
                if (prefixAttributes.Length <= 0) return typeof(TAggregateId).FullName.Replace(".", "").ToLower();
                var att = prefixAttributes[0];
                return att.Prefix;
        }
    }
}