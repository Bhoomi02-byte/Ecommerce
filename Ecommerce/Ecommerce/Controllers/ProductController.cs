using System.Security.Claims;
using Ecommerce.DTO;
using Ecommerce.Services;
using Ecommerce.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    [ApiController]
    [Route("api/product")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [Authorize(Roles = "Seller")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _productService.CreateAsync(dto, userId);
            if (result == null)
                return StatusCode(500, new ApiResponse(500, false, "Something went wrong", null));

            return Ok(new ApiResponse(200, true, "Product created successfully", result));

        }

        [Authorize(Roles = "Seller")]
        [HttpPut("{productId}")]
        public async Task<IActionResult> Update(int productId, [FromBody] ProductDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _productService.UpdateAsync(dto, userId,productId);
            if (result == null)
                return BadRequest( new ApiResponse(500, false, "Product not found or access denied", null));

            return Ok(new ApiResponse(200, true, result,null ));
        }

        [Authorize(Roles = "Seller")]
        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(int productId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _productService.DeleteAsync(userId, productId);

            if(result == null)  return BadRequest(new ApiResponse(400, false, "Product not found", null)); 
            return Ok(new ApiResponse(200, true, "Product deleted successfully", result));
        }

    }
}
