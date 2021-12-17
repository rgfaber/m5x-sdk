using M5x.DEC.Schema;

namespace M5x.DEC.TestKit.Tests.SuT;

[Topic(MyConfig.Hopes.MyHope)]
public record MyHope : Hope<MyPayload>
{
    public MyHope()
    {
        Payload = new MyPayload();
    }

    private MyHope(string aggregateId, string correlationId, MyPayload payload) : base(aggregateId, correlationId,
        payload)
    {
    }

    public static MyHope New(string id, string correlationId, MyPayload payload)
    {
        return new MyHope(id, correlationId, payload);
    }
}