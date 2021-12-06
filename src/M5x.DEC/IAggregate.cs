using System.Collections.Generic;
using M5x.DEC.Events;
using M5x.DEC.Schema;

namespace M5x.DEC
{
    public interface IAggregate<out TId>
        where TId : IIdentity
    {
        TId Id { get; }
    }
}