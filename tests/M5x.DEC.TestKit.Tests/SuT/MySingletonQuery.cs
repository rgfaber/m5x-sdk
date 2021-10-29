using M5x.DEC.Schema;

namespace M5x.DEC.TestKit.Tests.SuT
{
    public record MySingletonQuery : SingletonQuery
    {
        public MySingletonQuery()
        {
        }

        public MySingletonQuery(string id, string correlationId) : base(id, correlationId)
        {
        }
    }
}