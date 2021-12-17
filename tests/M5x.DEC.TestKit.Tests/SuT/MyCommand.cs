using M5x.DEC.Commands;
using M5x.DEC.Schema;

namespace M5x.DEC.TestKit.Tests.SuT;

public record MyCommand : Command<MyID>
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

    public MyCommand()
    {
    }

    public MyPayload Payload { get; set; }

    public static MyCommand New(AggregateInfo aggregateInfo, string correlationId, MyPayload payload)
    {
        return new MyCommand(MyID.With(aggregateInfo.Id), correlationId, payload);
    }
}