using System;
using System.Threading.Tasks;
using M5x.DEC.Persistence;
using M5x.DEC.Schema;
using Serilog;

namespace M5x.DEC.Infra.Redis
{
    public abstract class RedisWriter<TAggregateId, TFact, TReadModel> : IFactWriter<TAggregateId, TFact, TReadModel>
        where TAggregateId : IIdentity
        where TFact : IFact
        where TReadModel : IReadEntity
    {
        protected readonly ILogger Logger;

        public RedisWriter(ILogger logger)
        {
            Logger = logger;
        }

        public async Task HandleAsync(TFact fact)
        {
            try
            {
                await UpdateAsync(fact);
            }
            catch (Exception e)
            {
                Logger?.Error(e.Message);
            }
        }

        public abstract Task<TReadModel> UpdateAsync(TFact fact);
        public abstract Task<TReadModel> DeleteAsync(string id);
    }
}