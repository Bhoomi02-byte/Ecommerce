using Ecommerce.Services;
using StackExchange.Redis;

public class RedisService : IRedisService
{
    private readonly IDatabase _db;

    public RedisService(IConnectionMultiplexer connectionMultiplexer)
    {
        _db = connectionMultiplexer.GetDatabase();
    }

    public async Task AddToRecentlyViewedAsync(string userId, List<string> productIds)
    {
        var redisKey = $"recently_viewed:{userId}";
        foreach (var productId in productIds)
        {
            await _db.ListRemoveAsync(redisKey, productId); // avoid duplicates
            await _db.ListLeftPushAsync(redisKey, productId);
        }

        await _db.ListTrimAsync(redisKey, 0, 19); // keep only latest 20
    }

    public async Task<List<string>> GetRecentlyViewedAsync(string userId)
    {
        var redisKey = $"recently_viewed:{userId}";
        var values = await _db.ListRangeAsync(redisKey);
        return values.Select(v => v.ToString()).ToList();
    }
}
