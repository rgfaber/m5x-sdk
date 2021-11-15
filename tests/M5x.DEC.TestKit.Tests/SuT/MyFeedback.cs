using M5x.DEC.Schema;
using M5x.DEC.Schema.Common;

namespace M5x.DEC.TestKit.Tests.SuT
{
    public record MyFeedback : Feedback<Dummy>
    {
        public MyFeedback()
        {
        }

        private MyFeedback(AggregateInfo meta, string correlationId, Dummy payload) : base(meta, correlationId, payload)
        {
        }

        public static MyFeedback New(AggregateInfo meta, string correlationId, Dummy payload = null)
        {
            payload ??= Dummy.Empty;
            return new MyFeedback(meta, correlationId, payload);
        }
    }
}