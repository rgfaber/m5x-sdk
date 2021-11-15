using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using M5x.DEC.Persistence;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;
using M5x.DEC.Schema.Utils;
using M5x.Redis;
using StackExchange.Redis;

namespace M5x.DEC.Infra.Redis
{
    public abstract class RedisReader<TQuery, TPayload> : IModelReader<TQuery, TPayload>
        where TQuery : IQuery
        where TPayload : IPayload
    {
        public string DbName = AttributeUtils.GetDbName<TPayload>();

        protected RedisReader(IRedisDb redis)
        {
            Redis = redis;
        }

        protected IRedisDb Redis { get; }

        public Task<TPayload> GetByIdAsync(string id)
        {
            var key = new RedisKey(id);
            var res = Redis.GetKey<RedisDtoHash<TPayload>>(id);
            Guard.Against.Null(res, typeof(TPayload).PrettyPrint());
            return res.ToDto();
        }

        public abstract Task<IEnumerable<TPayload>> FindAllAsync(TQuery qry);
    }
}