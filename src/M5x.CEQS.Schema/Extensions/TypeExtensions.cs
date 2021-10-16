using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace M5x.CEQS.Schema.Extensions
{
    public static class TypeExtensions
    {
        
        public static string TableName(this IReadEntity entity)
        {
            var atts = (DbNameAttribute[]) entity.GetType().GetCustomAttributes(typeof(DbNameAttribute), true);
            if (atts.Length == 0) throw new Exception($"Attribute 'DBName' is not defined on {entity.GetType()}!");
            return atts[0].DbName;
        }

        
        
        
        private static readonly ConcurrentDictionary<Type, string> PrettyPrintCache =
            new();

        public static string PrettyPrint(this Type type)
        {
            return PrettyPrintCache.GetOrAdd(
                type,
                t =>
                {
                    try
                    {
                        return PrettyPrintRecursive(t, 0);
                    }
                    catch (Exception)
                    {
                        return t.Name;
                    }
                });
        }

        private static string PrettyPrintRecursive(Type type, int depth)
        {
            if (depth > 3) return type.Name;

            var nameParts = type.Name.Split('`');
            if (nameParts.Length == 1) return nameParts[0];

            var genericArguments = type.GetTypeInfo().GetGenericArguments();
            return !type.IsConstructedGenericType
                ? $"{nameParts[0]}<{new string(',', genericArguments.Length - 1)}>"
                : $"{nameParts[0]}<{string.Join(",", genericArguments.Select(t => PrettyPrintRecursive(t, depth + 1)))}>";
        }
    }
}