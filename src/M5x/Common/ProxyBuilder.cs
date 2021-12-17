using System;
using System.Collections.Generic;

namespace M5x.Common;

/// <summary>
///     Builder and cache of proxies
/// </summary>
internal static class ProxyBuilder
{
    // Generated Types Dictionary
    private static readonly IDictionary<string, Type> GeneratedTypes = new Dictionary<string, Type>();
    private static readonly object GenerateLock = new();

    public static Type BuildType<TInterface, TBuilder>()
        where TInterface : class
        where TBuilder : ITypeBuilder, new()
    {
        var typeName = GetTypeName<TInterface>(typeof(TBuilder).Name.Replace("ClassBuilder", ""));

        var type = TryGetType(typeName);
        if (type != null)
            return type;

        lock (GenerateLock)
        {
            type = TryGetType(typeName);
            if (type != null)
                return type;

            var builder = new TBuilder();

            type = builder.GenerateType(typeName);

            lock (GeneratedTypes)
            {
                GeneratedTypes[typeName] = type;
            }

            return type;
        }
    }


    private static Type TryGetType(string className)
    {
        lock (GeneratedTypes)
        {
            if (GeneratedTypes.TryGetValue(className, out var generatedType))
                return generatedType;
        }

        return null;
    }

    private static string GetTypeName<TType>(string classNameSuffix)
    {
        var iType = typeof(TType);
        if (iType.Name.StartsWith("I") && char.IsUpper(iType.Name, 1))
            return iType.Name.Substring(1) + classNameSuffix;
        return iType.Name + classNameSuffix;
    }
}