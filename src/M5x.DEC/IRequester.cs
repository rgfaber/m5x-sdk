using System.Threading;
using System.Threading.Tasks;
using M5x.DEC.Schema;

namespace M5x.DEC
{
    public interface IRequester<in THope, TFeedback>
        where THope : IHope
        where TFeedback : IFeedback
    {
        Task<TFeedback> RequestAsync(THope hope, CancellationToken cancellationToken=default);
    }
}