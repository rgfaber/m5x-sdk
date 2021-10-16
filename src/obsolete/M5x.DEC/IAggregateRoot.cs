using System;
using M5x.Schemas;
using M5x.Schemas.Commands;

namespace M5x.DEC
{
    public interface IAggregateRoot
    {
        IAggregateName Name { get; }
        long Version { get; }
        bool IsNew { get; }
        bool HasSourceId(ISourceID sourceId);
        IIdentity GetIdentity();
    }

    public interface IAggregateRoot<TIdentity> : IAggregateRoot,
        IAggregate<TIdentity>
        where TIdentity : IIdentity
    {
//        TIdentity Id { get; }
//        bool ValidateState(params Func<IAggregateRoot<TIdentity>, bool>[] validations);
    }
}