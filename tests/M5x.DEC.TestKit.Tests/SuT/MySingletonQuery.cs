using M5x.DEC.Schema;

namespace M5x.DEC.TestKit.Tests.SuT
{
    public record MySingletonQuery : SingletonQuery
    {
        public MySingletonQuery()
        {
        }

        public MySingletonQuery(string id) : base(id)
        {
        }

        public MySingletonQuery(string correlationId, string id) : base(correlationId, id)
        {
        }
    }
}