using M5x.Schemas;

namespace M5x.Publisher.Contract
{
    [IDPrefix("raf")]
    public record RafId: AggregateId<RafId>
    {
        public RafId(string value) : base(value)
        {
        }

        public static RafId CreateNew(string id)
        {
            return RafId.NewId();
        }
    }
}