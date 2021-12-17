namespace M5x.DEC.Snapshot;

public interface IHydrate<in TAggregateSnapshot>
    where TAggregateSnapshot : IAggregateSnapshot
{
    void Hydrate(TAggregateSnapshot aggregateSnapshot);
}