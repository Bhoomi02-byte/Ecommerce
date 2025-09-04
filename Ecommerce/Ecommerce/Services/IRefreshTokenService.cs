using Ecommerce.DTO;

namespace Ecommerce.Services
{
    public interface IRefreshTokenService
    {
        Task<string> RefreshTokenAsync(RefreshTokenDto dto);
    }
}
