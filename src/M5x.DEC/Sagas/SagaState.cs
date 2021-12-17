using System;
using M5x.DEC.Schema.Extensions;

namespace M5x.DEC.Sagas;

public abstract class SagaState<TSaga, TIdentity, TMessageApplier> : IMessageApplier<TSaga, TIdentity>
    where TMessageApplier : class, IMessageApplier<TSaga, TIdentity>
    where TSaga : IAggregateRoot<TIdentity>
    where TIdentity : ISagaId
{
    protected SagaState()
    {
        var me = this as TMessageApplier;
        if (me == null)
            throw new InvalidOperationException(
                $"MessageApplier of Type={GetType().PrettyPrint()} has a wrong generic argument Type={typeof(TMessageApplier).PrettyPrint()}.");
    }
}