using StackExchange.Redis;

namespace M5x.Redis
{
    public class RedisConnectionFactory : IRedisConnectionFactory
    {
        private ConfigurationOptions _options;

        public RedisConnectionFactory(ConfigurationOptions options)
        {
            _options = options;
        }

        public IConnectionMultiplexer Connect(ConfigurationOptions options = null)
        {
            if (options != null) _options = options;
            return ConnectionMultiplexer.Connect(_options);
        }
    }

    public interface IRedisConnectionFactory
    {
        IConnectionMultiplexer Connect(ConfigurationOptions options = null);
    }
}