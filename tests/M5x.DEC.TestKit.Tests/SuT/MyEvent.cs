using System;
using M5x.DEC.Events;
using M5x.DEC.Schema;

namespace M5x.DEC.TestKit.Tests.SuT
{
    [Topic(MyConfig.Facts.MyFact)]
    public sealed record MyEvent : Event<MyID>
    {
         public MyPayload Payload { get; set; }
        
        private MyEvent(AggregateInfo meta, string correlationId, MyPayload payload) : base(meta, correlationId)
        {
            Payload = payload;
        }

        public MyEvent()
        {
        }

        public static MyEvent New(MyCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (command.Payload == null) throw new Exception("Payload should not be nil");
            if (command.AggregateId == null) throw new Exception("AggregateId should not be nil");
            return new MyEvent(
                AggregateInfo.New(command.AggregateId.Value, -1,0), 
                command.CorrelationId, 
                command.Payload);
        }

        public override IEvent<MyID> WithAggregate(AggregateInfo meta, string correlationId)
        {
            return new MyEvent(meta, correlationId, Payload);
        }
    }
}