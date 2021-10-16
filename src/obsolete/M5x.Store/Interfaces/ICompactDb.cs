using System.Threading.Tasks;

namespace M5x.Store.Interfaces
{
    public interface ICompactDb
    {
        Task<XResponse> Compact(string name);
    }
}