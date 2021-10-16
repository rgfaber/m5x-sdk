using System.Threading.Tasks;
using EventFlow.Core;
using M5x.CEQS.Schema;

namespace M5x.CEQS
{
    public interface IModelWriter<TAggregateId, TFact, TReadModel> : IFactHandler<TAggregateId, TFact>
        where TReadModel : IReadEntity
        where TFact : IFact<TAggregateId>
        where TAggregateId : IIdentity
    {
        Task<TReadModel> UpdateAsync(TFact fact);
    }
}