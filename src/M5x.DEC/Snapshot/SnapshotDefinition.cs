using System;
using M5x.DEC.Schema.VersionedTypes;

namespace M5x.DEC.Snapshot
{
    public record SnapshotDefinition : VersionedTypeDefinition
    {
        public SnapshotDefinition(
            int version,
            Type type,
            string name)
            : base(version, type, name)
        {
        }
    }
}