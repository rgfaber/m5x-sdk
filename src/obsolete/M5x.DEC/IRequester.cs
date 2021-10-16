using System.Threading.Tasks;

namespace M5x.DEC
{
    public interface IRequester<in TReq, TRsp>
    {
        Task<TRsp> RequestAsync(TReq req);
    }
}