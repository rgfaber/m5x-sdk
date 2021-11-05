using System;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using M5x.DEC.Infra;
using M5x.DEC.Infra.Redis;
using M5x.DEC.Persistence;
using M5x.DEC.Schema.Extensions;
using M5x.Redis;


namespace M5x.DEC.TestKit.Tests.SuT.Infra.Redis
{
    
    
    public interface IMyRedisEventWriter :  IEtlWriter<MyEvent,MyReadModel>
    {
    }
    
    internal class MyRedisEtlEventWriter : RedisEtlEventWriter<MyEvent, MyReadModel>, IMyRedisEventWriter
    {
        // public async Task<MyReadModel> UpdateAsync(MyEvent evt)
        // {
        //     try
        //     {
        //         Guard.Against.BadEvent(evt);
        //         var doc = Redis.GetKey<RedisDtoHash<MyReadModel>>(evt.Meta.Id);
        //         var model = doc.ToDto().Result;
        //         model.Id = @evt.Meta.Id;
        //         model.Prev = @evt.Meta.Id;
        //         model.Content = @evt.Payload;
        //         model.Meta = evt.Meta;
        //          
        //         await doc.FromDto(model);
        //         return model;
        //     }
        //     catch (Exception e)
        //     {
        //         Console.WriteLine(e.InnerAndOuter());
        //         throw;
        //     }
        // }
        //
        // public async Task<MyReadModel> DeleteAsync(string id)
        // {
        //     var hash = Redis.GetKey<RedisDtoHash<MyReadModel>>(id);
        //     Model = await hash.ToDto();
        //     if (Model != null)
        //     {
        //         var isDeleted = await Redis.DeleteKey(id);
        //     }
        //     return Model;
        // }

        public MyRedisEtlEventWriter(IRedisDb redis,
            IInterpreter<MyReadModel, MyEvent> interpreter) : base(redis,
            interpreter)
        {
        }
    }
}