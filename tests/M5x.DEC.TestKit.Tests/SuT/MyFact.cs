using M5x.DEC.Schema;

namespace M5x.DEC.TestKit.Tests.SuT
{
    [Topic(MyConfig.Facts.MyFact)]
    public record MyFact : Fact<MyPayload>
    {
        public MyFact(AggregateInfo meta, string correlationId, MyPayload payload) : base(meta, correlationId, payload)
        {
        }

        public MyFact()
        {
        }

        public static MyFact New(AggregateInfo meta, string correlationId, MyPayload payload)
        {
            return new MyFact(meta, correlationId, payload);
        }
    }
}