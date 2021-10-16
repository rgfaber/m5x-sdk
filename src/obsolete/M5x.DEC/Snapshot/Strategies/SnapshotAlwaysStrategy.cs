namespace M5x.DEC.Snapshot.Strategies
{
    public class SnapshotAlwaysStrategy : ISnapshotStrategy
    {
        public static ISnapshotStrategy Instance => new SnapshotAlwaysStrategy();

        public bool ShouldCreateSnapshot(IAggregateRoot snapshotAggregateRoot)
        {
            return true;
        }
    }
}