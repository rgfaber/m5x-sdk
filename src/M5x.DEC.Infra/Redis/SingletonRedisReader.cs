using System.Collections.Generic;
using System.Threading.Tasks;
using M5x.DEC.Persistence;
using M5x.DEC.Schema;
using StackExchange.Redis;


namespace M5x.DEC.Infra.Redis
{
    public abstract class RedisSingleModelReader<TQuery, TPayload> : ISingleModelReader<TQuery, TPayload> 
        where TQuery : IQuery 
        where TPayload : IPayload
    {
        private readonly IConnectionMultiplexer _connection;
        protected RedisSingleModelReader(IConnectionMultiplexer connection)
        {
            _connection = connection;
        }

        public abstract Task<TPayload> GetByIdAsync(string id);
        public abstract Task<IEnumerable<TPayload>> FindAllAsync(TQuery qry);
    }
}