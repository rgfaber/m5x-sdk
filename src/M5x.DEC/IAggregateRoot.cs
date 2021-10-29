using System.Collections.Generic;
using M5x.DEC.Commands;
using M5x.DEC.Events;
using M5x.DEC.Schema;

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

    public interface IAggregateRoot<TIdentity> : IAggregateRoot, IAggregate<TIdentity>
        where TIdentity : IIdentity
    {

    }
}