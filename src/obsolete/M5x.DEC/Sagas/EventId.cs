using M5x.Schemas;

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