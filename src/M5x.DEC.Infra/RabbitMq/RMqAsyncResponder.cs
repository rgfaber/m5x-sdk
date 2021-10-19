using M5x.DEC.Commands;
using M5x.DEC.ExecutionResults;
using M5x.DEC.Schema;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace M5x.DEC.Infra.RabbitMq
{
    public abstract class RMqAsyncResponder<TAggregate, TID, THope, TCmd, TFeedback> : BackgroundService
        where TAggregate : IAggregateRoot<TID>
        where TID : IIdentity
        where TCmd : ICommand<TAggregate, TID, IExecutionResult>
        where THope : IHope
    {
        
        protected RMqAsyncResponder(
                IAsyncActor<TAggregate, TID, TCmd, THope, TFeedback> actor,
                ILogger logger)
        {}
        
        
        

            
    }
}