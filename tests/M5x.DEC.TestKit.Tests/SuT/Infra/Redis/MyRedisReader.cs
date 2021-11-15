using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using M5x.DEC.Infra.Redis;
using M5x.DEC.Persistence;
using M5x.Redis;

namespace M5x.DEC.TestKit.Tests.SuT.Infra.Redis
{
    public interface IMyRedisReader : ISingleModelReader<MyPagedQry, MyReadModel>
    {
    }

    public class MyRedisReader : RedisReader<MyPagedQry, MyReadModel>, IMyRedisReader
    {
        public MyRedisReader(IRedisDb redis) : base(redis)
        {
        }

        public override Task<IEnumerable<MyReadModel>> FindAllAsync(MyPagedQry qry)
        {
            var rsp = new List<MyReadModel>();
            var res = Redis.GetKey<RedisHash<string, MyReadModel>>(DbName);
            var enumerator = res.GetAsyncEnumerator();
            do
            {
                rsp.Add(enumerator.Current.Value);
            } while (enumerator.MoveNextAsync() != default);

            return Task.FromResult(rsp.AsEnumerable());
        }
    }
}