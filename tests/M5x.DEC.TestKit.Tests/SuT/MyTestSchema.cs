using M5x.DEC.Schema;
using M5x.DEC.Schema.Utils;

namespace M5x.DEC.TestKit.Tests.SuT
{
    public static class MyTestSchema
    {
        public const string TEST_CORRELATION_ID = "TEST_CORRELATION_ID";
        public static readonly MyID TestID = MyID.NewComb(GuidUtils.TEST_GUID);
        public static readonly MyReadModel Model = MyReadModel.New(TestID.Value, TestID.Value);
        public static readonly MyPayload Payload = MyBogus.Schema.Payload.Generate();
        public static readonly AggregateInfo Meta = MyBogus.Schema.Meta.Generate();
    }
}