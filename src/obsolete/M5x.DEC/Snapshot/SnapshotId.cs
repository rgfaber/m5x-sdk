using M5x.Schemas;

namespace M5x.DEC.Snapshot
{
    public record SnapshotId : Identity<SnapshotId>, ISnapshotId
    {
        public SnapshotId(string value)
            : base(value)
        {
        }
    }
}