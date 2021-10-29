using M5x.DEC.Commands;
using M5x.DEC.Schema;
using Microsoft.Extensions.Hosting;

namespace M5x.DEC
{
    public interface IResponder : IHostedService
    {
        string Topic { get; }
    }
    
    public interface IResponder<TID, THope, TCmd, TFeedback> :  IResponder
        where TID : IIdentity
        where THope : IHope
        where TCmd : ICommand<TID>
        where TFeedback : IFeedback
    {
    }
}