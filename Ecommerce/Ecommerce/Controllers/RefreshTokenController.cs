using Ecommerce.DTO;
using Ecommerce.Services;
using Ecommerce.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    [ApiController]
    [Route("generate")]
    public class RefreshTokenController : ControllerBase
    {
        private readonly IRefreshTokenService _token;

        public RefreshTokenController(IRefreshTokenService token)
        {
            _token = token;
        }

        [HttpPost("accesstoken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            var accessToken = await _token.RefreshTokenAsync(dto);

            if (accessToken == null)
                return Conflict(new ApiResponse(400, false, "Invalid or expired refresh token.", null));

            return Ok(new ApiResponse(200, true, "Access token refreshed successfully.",new
            { 
                AccessToken = accessToken
            }
            )); 
        }


    }
}
