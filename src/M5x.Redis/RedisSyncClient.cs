using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace M5x.Redis
{
    public class RedisSyncClient: IRedisSyncCient
    {
        private IRedis _redis;

        public RedisSyncClient(IRedis redis)
        {
            _redis = redis;
        }

        public Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None)
        {
            return _redis.PingAsync(flags);
        }

        public bool TryWait(Task task)
        {
            return _redis.TryWait(task);
        }

        public void Wait(Task task)
        {
            _redis.Wait(task);
        }

        public T Wait<T>(Task<T> task)
        {
            return _redis.Wait(task);
        }

        public void WaitAll(params Task[] tasks)
        {
            _redis.WaitAll(tasks);
        }

        public IConnectionMultiplexer Multiplexer => _redis.Multiplexer;

        public TimeSpan Ping(CommandFlags flags = CommandFlags.None)
        {
            return _redis.Ping(flags);
        }
    }

    public interface IRedisSyncCient : IRedis
    {
    }
}