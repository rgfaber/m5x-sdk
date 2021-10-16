using System.Collections.Generic;
using System.Threading.Tasks;

namespace M5x.Store.Interfaces
{
    public interface IReplicateDb
    {
        Task<Dictionary<string, XResponse>> Replicate(string name);
    }
}