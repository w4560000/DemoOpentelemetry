using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace DemoOpentelemetry.API2
{
    public class RedisUtility
    {
        private readonly IConnectionMultiplexer connectionMultiplexer;
        public RedisUtility(IConnectionMultiplexer _connectionMultiplexer)
        {
            this.connectionMultiplexer = _connectionMultiplexer;
        }

        public IDatabase cacheDB => this.connectionMultiplexer.GetDatabase();

        public RedLockFactory GetRedisLockFactory() => RedLockFactory.Create(new List<RedLockMultiplexer> { new RedLockMultiplexer(connectionMultiplexer) });
    }
}
