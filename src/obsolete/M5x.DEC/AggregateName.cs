using M5x.Schemas.ValueObjects;

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