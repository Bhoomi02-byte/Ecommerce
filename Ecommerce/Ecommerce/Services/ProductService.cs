using Ecommerce.Data;
using Ecommerce.DTO;
using Ecommerce.Models.Entities;
using Ecommerce.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        public ProductService(ApplicationDbContext context)
        {
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
            return "Product updated successfully";

        }
        public async Task<string> DeleteAsync(int userId, int productId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId && p.SellerId == userId);

            if (product == null)
                return null;

            _context.Products.Remove(product); //delete variant automatically by using cascade delete
            await _context.SaveChangesAsync();

            return "Product deleted successfully.";
        }


    }
}
