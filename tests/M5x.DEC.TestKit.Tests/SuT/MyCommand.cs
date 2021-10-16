using M5x.DEC.Commands;
using M5x.DEC.ExecutionResults;
using M5x.DEC.Schema;

namespace M5x.DEC.TestKit.Tests.SuT
{
    public record MyCommand: Command<MyAggregate,MyID,IExecutionResult>
    { 

        public MyCommand(MyID aggregateId, MyPayload payload) : base(aggregateId)
        {
            Payload = payload;
        }

        public MyCommand(MyID aggregateId, CommandId sourceId, MyPayload payload) : base(aggregateId, sourceId)
        {
            Payload = payload;
        }

        public MyCommand(MyPayload payload)
        {
            Payload = payload;
        }

        public MyCommand(MyID aggregateId, string correlationId, MyPayload payload) : base(aggregateId, correlationId)
        {
            Payload = payload;
        }

        public MyPayload Payload { get; set; }
        public static MyCommand New(AggregateInfo aggregateInfo, string correlationId, MyPayload payload)
        {
            return new(MyID.With(aggregateInfo.Id), correlationId, payload);
        }
        
        public MyCommand() {}
        
        
    }
}