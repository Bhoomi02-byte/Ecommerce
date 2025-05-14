using Ecommerce.DTO;

namespace Ecommerce.Services
{
    public interface IProductService
    {
        Task<object?> CreateAsync(ProductDto dto, int userId);
        Task<string> UpdateAsync(ProductDto dto, int userId, int productId);
        Task<string> DeleteAsync(int userId, int productId);
    }
}
