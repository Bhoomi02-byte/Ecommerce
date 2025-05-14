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
        [Authorize(Roles = "customer")]
        public async Task<IActionResult> AddToWishlist([FromBody] WishlistDto dto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var response = await _userService.AddToWishlistAsync(userId, dto);

            if (response == "Item already in wishlist.")
                return BadRequest(new ApiResponse(400, false, response, null));

            return Ok(new ApiResponse(200, true, response, null));
        }

        [HttpPost("cart")]
        public async Task<IActionResult> AddToCart([FromBody] CartDto dto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var response = await _userService.AddToCartAsync(userId, dto);
            if (response == null)
                return BadRequest(new ApiResponse(400, false, "Item do not add in wishlist.", null));

            return Ok(new ApiResponse(200, true, response, null));
        }

        [HttpDelete("cart/{cartItemId}")]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var response = await _userService.RemoveFromCartAsync(cartItemId, userId);

            if (response == "Item not found in Cart")
                return BadRequest(new ApiResponse(400, false, response, null));

            return Ok(new ApiResponse(200, true, response, null));
        }

        [HttpGet("cart")]

        public async Task<IActionResult> GetCart()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var cart = await _userService.GetCartByUserIdAsync(userId);

            if (cart == null)
                return BadRequest(new ApiResponse(400, false, "Cart is empty", cart));

            return Ok(new ApiResponse(200, true, "Cart fetched successfully", cart));
        }


    }
    
}
