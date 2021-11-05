using System.Threading.Tasks;
using Ardalis.GuardClauses;
using M5x.DEC.Events;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Utils;
using M5x.Redis;

namespace M5x.DEC.Infra.Redis
{
    public abstract class RedisEtlEventWriter<TEvent, TReadModel> 
        : EtlEventWriter<TEvent,TReadModel>
        where TEvent : IEvent<IIdentity>
        where TReadModel : IReadEntity
    {
        protected readonly IRedisDb Redis;


        protected override Task<TReadModel> ExtractAsync(TEvent @event)
        {
            Guard.Against.BadEvent(@event);
            var  hash = Redis.GetKey<RedisDtoHash<TReadModel>>(@event.Meta.Id);
           return hash.ToDto();
        }

        protected override Task<TReadModel> LoadAsync()
        {
            Guard.Against.Null(Model, nameof(Model));
            var  hash = Redis.GetKey<RedisDtoHash<TReadModel>>(Model.Id);
            hash.FromDto(Model);
            hash = Redis.GetKey<RedisDtoHash<TReadModel>>(Model.Id);
            return hash.ToDto();
        }

        public override async Task<TReadModel> DeleteAsync(string id)
        {
            Guard.Against.NullOrWhiteSpace(id, nameof(id));
            var  hash = Redis.GetKey<RedisDtoHash<TReadModel>>(Model.Id);
            Model = await hash.ToDto();
            var isDeleted = await Redis.DeleteKey(id);
            return isDeleted 
                ? Model 
                : default;
        }

        protected string DbName = AttributeUtils.GetDbName<TReadModel>();
        
        
        

        protected RedisEtlEventWriter(IRedisDb redis, IInterpreter<TReadModel, TEvent> interpreter) : base(interpreter)
        {
            Redis = redis;
        }
    }
}