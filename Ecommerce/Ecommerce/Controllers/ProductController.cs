using System.Security.Claims;
using Azure;
using Ecommerce.DTO;
using Ecommerce.Services;
using Ecommerce.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Ecommerce.Controllers
{
    [ApiController]
    [Route("api/product")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IRedisService _redisService;
        public ProductController(IProductService productService, IRedisService redisService)
        {
            _productService = productService;
            _redisService = redisService;
        }
        [Authorize(Roles = "Seller")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _productService.CreateAsync(dto, userId);
            if (result == null)
                return StatusCode(500, new ApiResponse(500, false, JsonHelper.GetMessage(121), null));

            return Ok(new ApiResponse(200, true, JsonHelper.GetMessage(122), result));

        }

        [Authorize(Roles = "Seller")]
        [HttpPut("{productId}")]
        public async Task<IActionResult> Update(int productId, [FromBody] ProductDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _productService.UpdateAsync(dto, userId,productId);
            if (result == null)
                return BadRequest( new ApiResponse(500, false, JsonHelper.GetMessage(123), null));

            return Ok(new ApiResponse(200, true, result,null ));
        }

        [Authorize(Roles = "Seller")]
        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(int productId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _productService.DeleteAsync(userId, productId);

            if(result == null)  return BadRequest(new ApiResponse(400, false, JsonHelper.GetMessage(124), null)); 
            return Ok(new ApiResponse(200, true, JsonHelper.GetMessage(125), result));
        }


        [HttpPost("filter")]
        public async Task<IActionResult> GetFilteredProducts([FromBody] ProductFilterDto filter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _productService.GetFilteredProductsAsync(userId,filter);
            return Ok(new ApiResponse(200, true, JsonHelper.GetMessage(164), response));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetProductById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
           
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound(new ApiResponse(404, false, "Product not found", null));

            if (!string.IsNullOrEmpty(userId))
            {
                await _redisService.AddToRecentlyViewedAsync(userId,  id.ToString());
            }

            return Ok(new ApiResponse(200, true, "Product fetched successfully", product));
        }


        [HttpGet("recently-viewed")]
        [Authorize]
        public async Task<IActionResult> GetRecentlyViewedProducts()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new ApiResponse(400, false, "User not authenticated.", null));

            var productIds = await _redisService.GetRecentlyViewedAsync(userId);

            if (productIds.Count == 0)
                return BadRequest(new ApiResponse(400, false, "No recently viewed products.", null));

            var latestProductId = productIds.First();

            var product = await _productService.GetProductByIdAsync(int.Parse(latestProductId));
            if (product == null)
                return NotFound(new ApiResponse(404, false, "Product not found", null));

            return Ok(new ApiResponse(200, true, "Latest recently viewed product", product));
        }

        [HttpPost("{productId}/upload-image")]
        public async Task<IActionResult> UploadImage(int productId, [FromForm] IFormFile image)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new ApiResponse(401, false, JsonHelper.GetMessage(170), null));
            }

            if (image == null || image.Length == 0)
                return BadRequest(new ApiResponse(400,false, JsonHelper.GetMessage(169), null));

            var result = await _productService.UploadImageAsync(productId, userId, image, Request);

            if (result == JsonHelper.GetMessage(168))
                return Ok(new ApiResponse(201, true, result, null));

            return BadRequest(new ApiResponse(400, false, result, null));
        }




    }
}
