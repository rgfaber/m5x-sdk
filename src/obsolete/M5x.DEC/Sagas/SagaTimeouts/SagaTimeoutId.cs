using M5x.DEC.Jobs;
using M5x.Schemas;

namespace M5x.DEC.Sagas.SagaTimeouts
{
    public record SagaTimeoutId : Identity<SagaTimeoutId>, IJobId
    {
        public SagaTimeoutId(string value) : base(value)
        {
        }
    }
}