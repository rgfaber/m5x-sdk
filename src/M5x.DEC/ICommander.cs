using System.Threading.Tasks;

namespace M5x.DEC
{
    public interface ICommander<TCmd, TRsp>
    {
        Task<TRsp> CommandAsync(TCmd cmd);
    }
}