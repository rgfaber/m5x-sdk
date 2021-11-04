using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using M5x.DEC.Events;
using M5x.DEC.Persistence;
using M5x.DEC.PubSub;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using Serilog;

namespace M5x.DEC.TestKit.Tests.SuT
{
    
    public interface IMyActor: IAsyncActor<MyID, MyCommand,  MyFeedback>
    {
    }
    
    internal class MyActor: AsyncActor<MyAggregate, MyID, MyCommand, MyFeedback>, IMyActor
    {
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

        public MyActor(
            IBroadcaster<MyID> caster,
            IMyEventStream aggregates,
            IDECBus bus,
            IEnumerable<IEventHandler<MyID, IEvent<MyID>>> handlers) : base(caster,
            aggregates,
            bus,
            handlers)
        {
        }
    }
}