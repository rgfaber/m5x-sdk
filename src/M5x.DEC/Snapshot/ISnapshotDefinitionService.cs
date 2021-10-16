using M5x.DEC.Core.VersionedTypes;

namespace M5x.DEC.Snapshot
{
    public interface
        ISnapshotDefinitionService : IVersionedTypeDefinitionService<SnapshotVersionAttribute, SnapshotDefinition>
    {
    }
}