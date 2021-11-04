using System.Collections.Generic;
using System.Threading.Tasks;
using M5x.DEC.Infra.Redis;
using M5x.DEC.Persistence;
using M5x.Redis;

namespace M5x.DEC.TestKit.Tests.SuT.Infra.Redis
{
    public interface IMySingletonRedisReader : ISingleModelReader<MySingletonQuery, MyReadModel>
    {
    }
    
    
    internal class MySingletonRedisReader: RedisReader<MySingletonQuery, MyReadModel>, IMySingletonRedisReader
    {
        public MySingletonRedisReader(IRedisDb redis) : base(redis)
        {
        }

        public override async Task<IEnumerable<MyReadModel>> FindAllAsync(MySingletonQuery qry)
        {
            var res = await GetByIdAsync(qry.Id);
            return new[] { res };
        }
    }
    
    
}