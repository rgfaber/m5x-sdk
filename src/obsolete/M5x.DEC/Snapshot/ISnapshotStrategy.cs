namespace M5x.DEC.Snapshot
{
    public interface ISnapshotStrategy
    {
        bool ShouldCreateSnapshot(IAggregateRoot snapshotAggregateRoot);
    }
}