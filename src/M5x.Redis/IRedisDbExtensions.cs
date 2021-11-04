using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace M5x.Redis
{
    public static class IRedisDbExtensions
    {
        public static bool IsConnected(this IRedisDb redisDb, string keyName)
        {
            if (redisDb == null) throw new ArgumentNullException("redisDb");
            return redisDb.DB.IsConnected(keyName);
        }

        public static Task<bool> KeyExists(this IRedisDb redisDb, string keyName, bool useKeyNameSpace = true)
        {
            if (redisDb == null) throw new ArgumentNullException("redisDb");
            var fullKeyName = useKeyNameSpace ? $"{redisDb.KeyNameSpace}:{keyName}" : keyName;
            return redisDb.DB.KeyExistsAsync(fullKeyName);
        }

        public static Task<long> KeyExists(this IRedisDb redisDb, IEnumerable<string> keyNames,
            bool useKeyNameSpace = true)
        {
            if (redisDb == null) throw new ArgumentNullException("redisDb");
            var keys = keyNames.Select(k => useKeyNameSpace ? $"{redisDb.KeyNameSpace}:{k}" : k).Cast<RedisKey>()
                .ToArray();
            return redisDb.DB.KeyExistsAsync(keys);
        }

        public static Task<bool> DeleteKey(this IRedisDb redisDb, string keyName, bool useKeyNameSpace = true)
        {
            if (redisDb == null) throw new ArgumentNullException("redisDb");
            var fullKeyName = useKeyNameSpace ? $"{redisDb.KeyNameSpace}:{keyName}" : keyName;
            return redisDb.DB.KeyDeleteAsync(fullKeyName);
        }

        public static Task<long> DeleteKey(this IRedisDb redisDb, IEnumerable<string> keyNames,
            bool useKeyNameSpace = true)
        {
            if (redisDb == null) throw new ArgumentNullException("redisDb");
            var keys = keyNames.Select(k => useKeyNameSpace ? $"{redisDb.KeyNameSpace}:{k}" : k).Cast<RedisKey>()
                .ToArray();
            return redisDb.DB.KeyDeleteAsync(keys);
        }
    }
}