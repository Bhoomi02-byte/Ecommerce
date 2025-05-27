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
        private readonly IUserService _userService;

        public OrderController(IUserService userService)
        { 
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderDto dto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var response = await _userService.PlaceOrderAsync( userId, dto);

            if(response == JsonHelper.GetMessage(133))
              return BadRequest(new ApiResponse(400, false, response, null));

            return Ok(new ApiResponse(200, true, response, null));
        }

    }
}
