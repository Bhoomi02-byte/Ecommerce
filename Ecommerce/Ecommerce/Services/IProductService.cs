using Ecommerce.DTO;
using Ecommerce.Models.Entities;
using Ecommerce.Utilities;

namespace Ecommerce.Services
{
    public interface IProductService
    {
        Task<object?> CreateAsync(ProductDto dto, int userId);
        Task<string> UpdateAsync(ProductDto dto, int userId, int productId);
        Task<string> DeleteAsync(int userId, int productId);
        Task<object?> GetFilteredProductsAsync(string userId,ProductFilterDto query);
        Task<Product?> GetProductByIdAsync(int id);
        Task<string> UploadImageAsync(int postId, int userId, IFormFile image, HttpRequest request);
       

        }
}
