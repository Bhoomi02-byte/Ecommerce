namespace Ecommerce.Services
{
    public interface IRedisService
    {
        Task AddToRecentlyViewedAsync(string userid,List<string> productIds);
        Task<List<string>> GetRecentlyViewedAsync(string userId);
    }

}
