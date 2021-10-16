using System.Threading.Tasks;
using EventFlow.Core;
using M5x.CEQS.Schema;

namespace M5x.CEQS
{
    public interface IFactHandler<TAggregateId, in TFact> 
        where TFact : IFact<TAggregateId> where TAggregateId : IIdentity
    {
        Task HandleAsync(TFact fact);
    }
    
    
    
    
    
}