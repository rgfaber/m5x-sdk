using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace M5x.CEQS.Schema.Extensions
{
    public static class AttributeExtensions
    {
        
        public static string TableName(this IReadEntity entity)
        {
            var atts = (DbNameAttribute[]) entity.GetType().GetCustomAttributes(typeof(DbNameAttribute), true);
            if (atts.Length == 0) throw new Exception($"Attribute 'DBName' is not defined on {entity.GetType()}!");
            return atts[0].DbName;
        }


    }
}