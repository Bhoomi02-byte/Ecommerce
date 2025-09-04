using Ecommerce.DTO;

namespace Ecommerce.Services
{
    public interface IVariantService
    {
        Task<object?> CreateAsync(List<VariantDto> dto, int userId, int productId);
        Task<object?> UpdateGroupAsync(int productId, VariantDto variantDto, int userId);
        Task<string> DeleteVariantByIdAsync(int variantId, int userId);
        Task<object> GetGroupAsync(int productId);
    }
}
