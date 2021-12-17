namespace M5x.DEC.Snapshot.Strategies;

public class SnapshotEveryFewVersionsStrategy : ISnapshotStrategy
{
    public const int DefautSnapshotAfterVersions = 50;

    public SnapshotEveryFewVersionsStrategy(
        int snapshotAfterVersions)
    {
        SnapshotAfterVersions = snapshotAfterVersions;
    }

    public int SnapshotAfterVersions { get; }

    public static ISnapshotStrategy Default { get; } = With();


    public bool ShouldCreateSnapshot(IAggregateRoot snapshotAggregateRoot)
    {
        if (snapshotAggregateRoot.Version % SnapshotAfterVersions == 0 && !snapshotAggregateRoot.IsNew) return true;

        return false;
    }

    public static SnapshotEveryFewVersionsStrategy With(
        int snapshotAfterVersions = DefautSnapshotAfterVersions)
    {
        return new SnapshotEveryFewVersionsStrategy(
            snapshotAfterVersions);
    }
}