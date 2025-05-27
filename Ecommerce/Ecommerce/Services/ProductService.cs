using System.Linq;
using Ecommerce.Data;
using Ecommerce.DTO;
using Ecommerce.Models.Entities;
using Ecommerce.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services
{
    public class ProductService : IProductService
    {
        private readonly IRedisService _redisService;
        private readonly ApplicationDbContext _context;
        public ProductService(IRedisService redisService,ApplicationDbContext context)
        {
            _redisService = redisService;
            _context = context;

        }
        public async Task<object?> CreateAsync(ProductDto dto, int userId)
        {
            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                DressStyle = dto.DressStyle,
                ImageUrl = dto.ImageUrl,
                Description = dto.Description,
                Category = dto.Category,
                SellerId = userId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var variantEntities = new List<Variant>();

            foreach (var sizeGroup in dto.Variants)
            {
                foreach (var colorOption in sizeGroup.Options)
                {
                    variantEntities.Add(new Variant
                    {
                        Size = sizeGroup.Size,
                        Color = colorOption.Color,
                        Count = colorOption.Count,
                        ProductId = product.Id
                    });
                }
            }
            _context.Variants.AddRange(variantEntities);
            await _context.SaveChangesAsync();

            return new
            {
                product.Id,
                product.Name,
                product.Price,
                product.DressStyle,
                product.Category,
                product.ImageUrl,
                product.Description,
                product.SellerId,
                Variants = variantEntities.Select(v => new
                {
                    v.Size,
                    v.Color,
                    v.Count
                })
            };
        }

        public async Task<string> UpdateAsync(ProductDto dto, int userId, int productId)
        {
            var product = await _context.Products
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.Id == productId && p.SellerId == userId);

            if (product == null) { return null; }

            product.Name = dto.Name;
            product.Price = dto.Price;
            product.Description = dto.Description;
            product.ImageUrl = dto.ImageUrl;
            product.DressStyle = dto.DressStyle;
            product.Category = dto.Category;

            _context.Variants.RemoveRange(product.Variants);
            var newVariants = new List<Variant>();
            foreach (var sizeGroup in dto.Variants)
            {
                foreach (var colorOption in sizeGroup.Options)
                {
                    newVariants.Add(new Variant
                    {
                        ProductId = product.Id,
                        Size = sizeGroup.Size,
                        Color = colorOption.Color,
                        Count = colorOption.Count
                    });
                }
            }
            product.Variants = newVariants;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return JsonHelper.GetMessage(126);

        }
        public async Task<string> DeleteAsync(int userId, int productId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId && p.SellerId == userId);

            if (product == null)
                return null;

            _context.Products.Remove(product); //delete variant automatically by using cascade delete
            await _context.SaveChangesAsync();

            return JsonHelper.GetMessage(125);
        }
        public async Task<object?> GetFilteredProductsAsync(string userId,ProductFilterDto query)
        {
            var productsQuery = _context.Products
                .Include(p => p.Variants)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
                productsQuery = productsQuery.Where(p => p.Name.Contains(query.Search));

            if (!string.IsNullOrWhiteSpace(query.Category))
                productsQuery = productsQuery.Where(p => p.Category == query.Category);

            if (query.MinPrice.HasValue)
                productsQuery = productsQuery.Where(p => p.Price >= query.MinPrice);

            if (query.MaxPrice.HasValue)
                productsQuery = productsQuery.Where(p => p.Price <= query.MaxPrice);

            if (!string.IsNullOrWhiteSpace(query.Size))
                productsQuery = productsQuery.Where(p => p.Variants.Any(v => v.Size == query.Size));

            if (!string.IsNullOrWhiteSpace(query.Color))
                productsQuery = productsQuery.Where(p => p.Variants.Any(v => v.Color == query.Color));

         
            productsQuery = query.SortBy switch
            {
                "price_asc" => productsQuery.OrderBy(p => p.Price),
                "price_desc" => productsQuery.OrderByDescending(p => p.Price),
                _ => productsQuery.OrderBy(p => p.Name)
            };
          
            int totalCount = await productsQuery.CountAsync();
            var products = await productsQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Category,
                    p.ImageUrl,
                    p.Description,
                    p.DressStyle,
                    Variants = p.Variants.Select(v => new
                    {
                        v.Id,
                        v.Size,
                        v.Color,
                        v.Count
                    })
                })
                .ToListAsync();

            var productIds = products.Select(p => p.Id.ToString()).ToList();
            await _redisService.AddToRecentlyViewedAsync(userId, productIds);

            var result = new
            {
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize),
                Items = products
            };

            return result;
        }

        public async Task<List<object>> GetProductsByIdsAsync(List<string> ids)
        {
            var intIds = ids.Select(int.Parse).ToList();

            var products = await _context.Products
                .Include(p => p.Variants)
                .Where(p => intIds.Contains(p.Id)) 
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Category,
                    p.ImageUrl,
                    p.Description,
                    p.DressStyle,
                    Variants = p.Variants.Select(v => new
                    {
                        v.Id,
                        v.Size,
                        v.Color,
                        v.Count
                    })
                })
                .ToListAsync();

            return products.Cast<object>().ToList(); 
        }



    }
}
