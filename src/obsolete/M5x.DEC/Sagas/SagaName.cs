using M5x.Schemas;
using M5x.Schemas.ValueObjects;

namespace M5x.DEC.Sagas
{
    public interface ISagaName : IIdentity
    {
    }

    public record SagaName : SingleValueObject<string>, ISagaName
    {
        public SagaName(string value)
            : base(value)
        {
        }
    }
}