namespace M5x.DEC.Snapshot.Strategies
{
    public class SnapshotNeverStrategy : ISnapshotStrategy
    {
        public static ISnapshotStrategy Instance => new SnapshotNeverStrategy();

        public bool ShouldCreateSnapshot(IAggregateRoot snapshotAggregateRoot)
        {
            return false;
        }
    }
}