using Ecommerce.Services;
using StackExchange.Redis;

public class RedisService : IRedisService
{
    private readonly IDatabase _db;

    public RedisService(IConnectionMultiplexer connectionMultiplexer)
    {
        _db = connectionMultiplexer.GetDatabase();
    }

    public async Task AddToRecentlyViewedAsync(string userId, string productId)
    {
        var redisKey = $"recently_viewed:{userId}";

        await _db.ListRemoveAsync(redisKey, productId);   
        await _db.ListLeftPushAsync(redisKey, productId); 
        //await _db.ListTrimAsync(redisKey, 0, 19);         // Keep only latest 20
    }


    public async Task<List<string>> GetRecentlyViewedAsync(string userId)
    {
        var redisKey = $"recently_viewed:{userId}";
        var values = await _db.ListRangeAsync(redisKey);
        return values.Select(v => v.ToString()).ToList();
    }
}
