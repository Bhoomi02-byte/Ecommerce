using Ecommerce.Data;
using Ecommerce.DTO;
using Ecommerce.Models.Entities;
using Ecommerce.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services
{
    public class VariantService : IVariantService
    {
        private readonly ApplicationDbContext _context;
        public VariantService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<object?> CreateAsync(List<VariantDto> dto, int userId, int productId)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null) { return null; }
            if (product.SellerId != userId)
            {
                return JsonHelper.GetMessage(157);
            }

            var variants = dto.SelectMany(d => d.Options.Select(a => new Variant
            {
                Size = d.Size,
                Color = a.Color,
                Count = a.Count,
                ProductId = productId
            })).ToList();

            await _context.Variants.AddRangeAsync(variants);
            await _context.SaveChangesAsync();

            return JsonHelper.GetMessage(156);
        }

        public async Task<object?> UpdateGroupAsync(int productId, VariantDto variantDto, int userId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return null;

            if (product.SellerId != userId)
                return JsonHelper.GetMessage(158);

            var oldVariants = _context.Variants
                .Where(v => v.ProductId == productId && v.Size == variantDto.Size);

            _context.Variants.RemoveRange(oldVariants);

            var newVariants = variantDto.Options.Select(opt => new Variant
            {
                ProductId = productId,
                Size = variantDto.Size,
                Color = opt.Color,
                Count = opt.Count
            });

            await _context.Variants.AddRangeAsync(newVariants);
            await _context.SaveChangesAsync();

            return JsonHelper.GetMessage(159);
        }

        public async Task<string> DeleteVariantByIdAsync(int variantId, int userId)
        {
            var variant = await _context.Variants
                .Include(v => v.Product)
                .FirstOrDefaultAsync(v => v.Id == variantId);

            if (variant == null)
                return JsonHelper.GetMessage(124);

            if (variant.Product.SellerId != userId)
                return JsonHelper.GetMessage(160);

            _context.Variants.Remove(variant);
            await _context.SaveChangesAsync();

            return JsonHelper.GetMessage(161);
        }

        public async Task<object> GetGroupAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return null;

            var groupedVariants = await _context.Variants
                .Where(v => v.ProductId == productId)
                .GroupBy(v => v.Size)
                .Select(g => new VariantDto
                {
                    Size = g.Key,
                    Options = g.Select(v => new ColorOptionDto
                    {  
                        Color = v.Color,
                        Count = v.Count
                    }).ToList()
                })
                .ToListAsync();

            return groupedVariants;
        }





    }
}
