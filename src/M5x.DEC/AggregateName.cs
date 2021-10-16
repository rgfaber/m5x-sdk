using M5x.DEC.Schema.ValueObjects;

namespace M5x.DEC
{
    public record AggregateName : SingleValueObject<string>, IAggregateName
    {
        public AggregateName(string value)
            : base(value)
        {
        }
    }
}