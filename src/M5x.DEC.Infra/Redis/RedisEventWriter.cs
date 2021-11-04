using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using M5x.DEC.Events;
using M5x.DEC.Persistence;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Utils;
using M5x.Redis;
using Serilog;
using StackExchange.Redis;

namespace M5x.DEC.Infra.Redis
{
    public abstract class RedisEventWriter<TAggregateId, TEvent, TReadModel> 
        : IEventWriter<TAggregateId, TEvent, TReadModel>
        where TAggregateId : IIdentity
        where TEvent : IEvent<TAggregateId>
        where TReadModel : IReadEntity
    {
        protected IRedisDb Redis { get; }


        public RedisEventWriter(IRedisDb redis)
        {
            Redis = redis;
        }

        public Task HandleAsync(TEvent evt)
        {
            return UpdateAsync(evt);
        }

        protected string DbName = AttributeUtils.GetDbName<TReadModel>();

        /// <summary>
        /// When implementing this method, it is important to write the value using Unbox()
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        public abstract Task<TReadModel> UpdateAsync(TEvent evt);
        public Task<TReadModel> DeleteAsync(string id)
        {
            var model = Redis.GetKey<RedisDtoHash<TReadModel>>(id);
            var saved = model.ToDto().Result;
            model.DeleteKey();
            return Task.FromResult(saved);
        }
        
    }
}