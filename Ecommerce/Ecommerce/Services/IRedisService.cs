namespace Ecommerce.Services
{
    public interface IRedisService
    {
        Task AddToRecentlyViewedAsync(string userId, string productId);
        Task<List<string>> GetRecentlyViewedAsync(string userId);
    }

}
