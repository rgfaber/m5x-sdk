using M5x.DEC.Schema;

namespace M5x.DEC.Sagas
{
    public record EventId : Identity<EventId>, IEventId
    {
        public EventId(string value)
            : base(value)
        {
        }
    }
}