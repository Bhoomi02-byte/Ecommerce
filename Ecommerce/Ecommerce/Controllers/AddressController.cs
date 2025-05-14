using Azure;
using Ecommerce.Data;
using Ecommerce.DTO;
using Ecommerce.Models.Entities;
using Ecommerce.Services;
using Ecommerce.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/address")]
public class AddressController : ControllerBase
{
    private readonly IUserService _userService;
    public AddressController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> AddAddress([FromBody] AddressDto dto)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var response = await _userService.AddAddressAsync(dto,userId);
        return Ok(new ApiResponse(200, true, response, null));
    }

    [HttpGet]
    public async Task<IActionResult> GetAddresses()
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        var response = await _userService.GetAddressAsync(userId);
       return Ok(new ApiResponse(200, true, "Address fetch successfully", response));
      
    }
}
