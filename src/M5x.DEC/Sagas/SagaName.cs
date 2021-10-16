using M5x.DEC.Schema;
using M5x.DEC.Schema.ValueObjects;

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