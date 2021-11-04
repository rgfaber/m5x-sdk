using System;
using System.CodeDom;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Bogus.DataSets;
using M5x.DEC.Infra.Redis;
using M5x.DEC.Persistence;
using M5x.DEC.Schema.Extensions;
using M5x.Redis;
using StackExchange.Redis;

namespace M5x.DEC.TestKit.Tests.SuT.Infra.Redis
{
    
    
    public interface IMyRedisEventWriter :  IEventWriter<MyID, MyEvent,MyReadModel>
    {
    }
    
    internal class MyRedisEventEventWriter : RedisEventWriter<MyID, MyEvent, MyReadModel>, IMyRedisEventWriter
    {
        public MyRedisEventEventWriter(IRedisDb redis) : base(redis)
        {
        }

        public override async Task<MyReadModel> UpdateAsync(MyEvent evt)
        {
            try
            {
                Guard.Against.Null(evt, nameof(evt));
                Guard.Against.Null(evt.Meta, nameof(evt.Meta));
                Guard.Against.NullOrWhiteSpace(evt.Meta.Id, nameof(evt.Meta.Id));
                var doc = Redis.GetKey<RedisDtoHash<MyReadModel>>(evt.Meta.Id);
                var model = doc.ToDto().Result;
                model.Id = @evt.Meta.Id;
                model.Prev = @evt.Meta.Id;
                model.Content = @evt.Payload;
                model.Meta = evt.Meta;
                 
                await doc.FromDto(model);
                return model;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerAndOuter());
                throw;
            }
        }
    }
}