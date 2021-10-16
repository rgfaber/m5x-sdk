using System.Threading;
using System.Threading.Tasks;
using M5x.DEC.Schema;

namespace M5x.DEC
{
    public interface IFactEmitter<TAggregateId, TFact>
        where TAggregateId : IIdentity
        where TFact : IFact
    {
        Task EmitAsync(TFact fact, CancellationToken cancellationToken=default);
    }
}