using System;
using System.Collections.Generic;
using System.Reflection;
using M5x.DEC.Schema.Extensions;
using M5x.DEC.Schema.ValueObjects;

namespace M5x.DEC.Schema.VersionedTypes;

public abstract record VersionedTypeDefinition : ValueObject
{
    protected VersionedTypeDefinition(
        int version,
        Type type,
        string name)
    {
        Version = version;
        Type = type;
        Name = name;
    }

    public int Version { get; }
    public Type Type { get; }
    public string Name { get; }

    public override string ToString()
    {
        var assemblyName = Type.GetTypeInfo().Assembly.GetName();
        return $"{Name} v{Version} ({assemblyName.Name} - {Type.PrettyPrint()})";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Version;
        yield return Type;
        yield return Name;
    }
}