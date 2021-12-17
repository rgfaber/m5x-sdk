using System;
using M5x.DEC.Schema.VersionedTypes;

namespace M5x.DEC.Snapshot;

[AttributeUsage(AttributeTargets.Class)]
public class SnapshotVersionAttribute : VersionedTypeAttribute
{
    public SnapshotVersionAttribute(
        string name,
        int version)
        : base(name, version)
    {
    }
}