using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using M5x.DEC.PubSub;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using Serilog;

namespace M5x.DEC.TestKit.Tests.SuT
{
    
    public interface IMyActor: IAsyncActor<MyAggregate, MyID, MyCommand, MyHope, MyFeedback>
    {
    }
    
    internal class MyActor: AsyncActor<MyAggregate, MyID, MyCommand, MyHope, MyFeedback, MyEvent, MyFact>, IMyActor
    {
        public MyActor(IMyEventStream aggregates, IDECBus bus, 
            IEnumerable<IEventHandler<MyID, MyEvent>> handlers, 
            IMyEmitter emitter, ILogger logger) : base(aggregates, bus, handlers, emitter, logger)
        {
        }

        protected override MyCommand ToCommand(MyHope hope)
        {
            return MyCommand.New(AggregateInfo.New(hope.AggregateId, -1, 0), MyTestSchema.TEST_CORRELATION_ID, hope.Payload);
        }

        protected override async Task<MyFeedback> Act(MyCommand cmd)
        {
            AggregateInfo meta = AggregateInfo.New(cmd.AggregateId.Value, -1, 0);
            var rsp = MyFeedback.New(meta, cmd.CorrelationId);
            try
            {
                var root = await Aggregates.GetByIdAsync(cmd.AggregateId);
                if (root != null)
                {
                    var res = root.Execute(cmd);
                    if (res.IsSuccess)
                        await Aggregates.SaveAsync(root);
                    rsp.Meta = AggregateInfo.New(root.Id.Value, root.Version,(int) root.Status);
                }
            }
            catch (Exception e)
            {
                rsp.ErrorState.Errors.Add("MyActor.Error", e.AsApiError());
            }
            return rsp;
        }

        protected override MyFact ToFact(MyEvent @event)
        {
            throw new System.NotImplementedException();
        }
    }
}