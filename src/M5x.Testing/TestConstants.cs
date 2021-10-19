using System;
using M5x.DEC.Schema.Utils;

namespace M5x.Testing
{
    public static class TestConstants
    {
        public const string CORRELATION_ID = "TEST_CORRELATION_ID";
        public const  string Id = GuidUtils.TEST_GUID;
        public static readonly Guid Guid = Guid.Parse(GuidUtils.TEST_GUID);
        public const string TEST_HOPE_TOPIC = "Test.DoIt";
        public const string GroupName = "TEST_GROUP";
    }
}