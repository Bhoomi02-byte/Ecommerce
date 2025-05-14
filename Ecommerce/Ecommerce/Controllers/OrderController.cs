using System.Security.Claims;
using Ecommerce.DTO;
using Ecommerce.Services;
using Ecommerce.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderController : Controller
    {
        private readonly UserService _userService;

        public OrderController(UserService userService)
        { 
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderDto dto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var response = await _userService.PlaceOrderAsync( userId, dto);

            if(response == "Cart is empty") return BadRequest(new ApiResponse(400, false, response, null));

            return Ok(new ApiResponse(200, true, response, null));
        }

    }
}
