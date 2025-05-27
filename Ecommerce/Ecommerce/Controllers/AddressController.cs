using Azure;
using Ecommerce.Data;
using Ecommerce.DTO;
using Ecommerce.Models.Entities;
using Ecommerce.Services;
using Ecommerce.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Net;
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

        var response = await _userService.AddAddressAsync(dto, userId);

        if(response == null)
          return BadRequest(new ApiResponse(400, false, JsonHelper.GetMessage(163), null));
        
        return Ok(new ApiResponse(200, true, response, null));
    }

    [HttpGet]
    public async Task<IActionResult> GetAddresses()
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        var response = await _userService.GetAddressAsync(userId);
        if (response == JsonHelper.GetMessage(127))
            return BadRequest(new ApiResponse(400, false, response, null));

        return Ok(new ApiResponse(200, true, response, null));

    }

    [HttpPut("{addressId}")]
    public async Task<IActionResult> UpdateAddresses(int addressId, AddressDto dto)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        var result = await _userService.UpdateAddressAsync(userId, dto, addressId);

        if (result == JsonHelper.GetMessage(154)) 
            return BadRequest(new ApiResponse(400, false, result, null));

        if (result == JsonHelper.GetMessage(127))
            return NotFound(new ApiResponse(404, false, result, null));

        return Ok(new ApiResponse(200, true, result, null));

    }

    [HttpDelete("{addressId}")]
    public async Task<IActionResult> DeleteAddress(int addressId)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        var result = await _userService.DeleteAddressAsync(userId, addressId);

        if (result == JsonHelper.GetMessage(127)) 
            return NotFound(new ApiResponse(404, false, result, null));

        return Ok(new ApiResponse(200, true, result, null));
    }
}
