using System;
using System.Linq.Expressions;
using M5x.DEC.Schema;

namespace M5x.DEC.Sagas.AggregateSaga;

public interface IAggregateSagaManager
{
}

public interface IAggregateSagaManager<TAggregateSaga, TIdentity, TSagaLocator> : IAggregateSagaManager
    where TAggregateSaga : IAggregateSaga<TIdentity>
    where TIdentity : SagaId<TIdentity>, IIdentity
    where TSagaLocator : class, ISagaLocator<TIdentity>, new()
{
    Expression<Func<TAggregateSaga>> SagaFactory { get; }
}