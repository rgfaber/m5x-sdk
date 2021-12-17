using System;

namespace M5x.DEC.Schema.VersionedTypes;

public abstract class VersionedTypeAttribute : Attribute
{
    protected VersionedTypeAttribute(
        string name,
        int version)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
        if (version <= 0) throw new ArgumentOutOfRangeException(nameof(version), "Version must be positive");

        Name = name;
        Version = version;
    }

    public string Name { get; }
    public int Version { get; }
}