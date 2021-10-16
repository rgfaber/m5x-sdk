using M5x.Schemas;
using M5x.Schemas.ValueObjects;

namespace M5x.DEC.Sagas
{
    public abstract record SagaId<T> : SingleValueObject<string>, ISagaId
        where T : SagaId<T>
    {
        protected SagaId(string value)
            : base(value)
        {
        }
    }

    public interface ISagaId : IIdentity
    {
    }
}