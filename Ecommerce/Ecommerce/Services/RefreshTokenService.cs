using Ecommerce.Data;
using Ecommerce.DTO;
using Ecommerce.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services
{
    public class RefreshTokenService:IRefreshTokenService
    {
        private readonly ApplicationDbContext _context;
        private readonly GenerateJwtToken _token;

        public RefreshTokenService(ApplicationDbContext context, GenerateJwtToken token)
        {
            _context = context;
            _token = token;
        }
        public async Task<string?> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var storedToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == dto.RefreshToken);

            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiryDate < DateTime.UtcNow)
            {
                return null;
            }

            var newAccessToken = _token.GenerateToken(storedToken.User, "Access");
            return newAccessToken;
            
         }

    }
}
