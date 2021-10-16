using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using M5x.DEC;
using M5x.DEC.Persistence;
using M5x.DEC.PubSub;
using M5x.Publisher.Contract;

namespace M5x.Stan.Subscriber.Cli
{
    public interface ITestRafActor : IActor<RafId, TestRafReq, TestRafRsp, TestRaf> {}
    
    
    internal class TestRafActor: Actor<RafRoot, RafId, TestRafReq, TestRafRsp, TestRaf, RafTested>, ITestRafActor
    {
        public TestRafActor(IRafEventRepo eventRepo,
            IAggregateSubscriber subscriber,
            IEnumerable<IRafTestedEmitter> handlers,
            IMapper mapper) : base(eventRepo,
            subscriber,
            handlers,
            mapper)
        {
        }

        protected override TestRaf CreateCommand(TestRafReq req)
        {
            return TestRaf.CreateNew( RafId.CreateNew(req.AggregateId), req);
        }

        protected override async Task<TestRafRsp> Act(TestRaf cmd)
        {
            var rsp = new TestRafRsp(cmd.CorrelationId);
            if (cmd != null)
            {
                var root = RafRoot.CreateNew(cmd.AggregateId);
                root.Execute(cmd);
            }
            return rsp;
        }
    }
}