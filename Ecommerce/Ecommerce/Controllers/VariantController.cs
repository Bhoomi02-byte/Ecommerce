using System.Security.Claims;
using Ecommerce.DTO;
using Ecommerce.Services;
using Ecommerce.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace Ecommerce.Controllers
{
    [ApiController]
    [Route("api/product/variant")]
    public class VariantController :ControllerBase
    {
        private readonly IVariantService _variantService;
        public VariantController(IVariantService variantService)
        {
            _variantService = variantService;
        }

        [HttpPost("{productId}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Create([FromBody] List<VariantDto> dto, int productId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var result = await _variantService.CreateAsync(dto, userId, productId);
            if (result == null)
                return BadRequest(new ApiResponse(500, false, JsonHelper.GetMessage(124), null));

            if(result == JsonHelper.GetMessage(157))
                return BadRequest(new ApiResponse(500, false, JsonHelper.GetMessage(157), null));

            return Ok(new ApiResponse(200, true, JsonHelper.GetMessage(156), null));

        }

        [HttpPut("{productId}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Update(int productId, [FromBody] VariantDto dto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var response = await _variantService.UpdateGroupAsync(productId, dto, userId);

            if (response == null)
                return BadRequest(new ApiResponse(500, false, JsonHelper.GetMessage(124), null));

            if (response == JsonHelper.GetMessage(158))
                return BadRequest(new ApiResponse(500, false, JsonHelper.GetMessage(158), null));

            return Ok(new ApiResponse(200, true, JsonHelper.GetMessage(161), null));
           
        }

        [HttpDelete("{variantId}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> DeleteVariantById(int variantId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var response = await _variantService.DeleteVariantByIdAsync(variantId, userId);

            if (response == null)
                return BadRequest(new ApiResponse(500, false, JsonHelper.GetMessage(124), null));

            if(response == JsonHelper.GetMessage(160))
                return BadRequest(new ApiResponse(500, false, JsonHelper.GetMessage(160), null));

            return Ok(new ApiResponse(200, true, JsonHelper.GetMessage(161), null));

        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetGroup(int productId)
        {
            var response = await _variantService.GetGroupAsync(productId);
            if (response == null)
                return BadRequest(new ApiResponse(500, false, JsonHelper.GetMessage(124), null));

            return Ok(new ApiResponse(200, true, JsonHelper.GetMessage(162), response));

        }



       
    }
}
