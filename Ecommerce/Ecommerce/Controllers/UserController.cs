using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.DTO;
using Ecommerce.Utilities;
using Ecommerce.Services;
using Ecommerce.Models.Entities;
using Azure;

namespace Ecommerce.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("wishlist")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddToWishlist([FromBody] WishlistDto dto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var response = await _userService.AddToWishlistAsync(userId, dto);

            if (response == JsonHelper.GetMessage(130))
                return BadRequest(new ApiResponse(400, false, response, null));

            return Ok(new ApiResponse(200, true, response, null));
        }

        [HttpPost("cart")]
        public async Task<IActionResult> AddToCart([FromBody] CartDto dto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var response = await _userService.AddToCartAsync(userId, dto);
            if (response == null)
                return BadRequest(new ApiResponse(400, false, JsonHelper.GetMessage(131), null));

            return Ok(new ApiResponse(200, true, response, null));
        }

        [HttpDelete("cart/{Id}")]
        public async Task<IActionResult> RemoveFromCart(int Id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var response = await _userService.RemoveFromCartAsync(Id, userId);

            if (response == JsonHelper.GetMessage(132))
                return BadRequest(new ApiResponse(400, false, response, null));

            return Ok(new ApiResponse(200, true, response, null));
        }


        [HttpGet("cart")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetCart()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var cart = await _userService.GetCartByUserIdAsync(userId);

            if (cart == null)
                return BadRequest(new ApiResponse(404, false, JsonHelper.GetMessage(133), null));

            return Ok(new ApiResponse(200, true, JsonHelper.GetMessage(134), cart));
        }


    }
    
}
