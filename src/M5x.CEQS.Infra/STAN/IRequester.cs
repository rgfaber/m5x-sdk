using System.Threading.Tasks;
using M5x.CEQS.Schema;

namespace M5x.CEQS.Infra.STAN
{
    public interface IRequester<in THope, TFeedback>
    where THope: IHope
    where TFeedback: IFeedback
    {
        Task<TFeedback> RequestAsync(THope req);
    }
}