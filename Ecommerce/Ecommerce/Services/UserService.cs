using System.Security.Claims;
using Ecommerce.Data;
using Ecommerce.DTO;
using Ecommerce.Models.Entities;
using Ecommerce.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services
{
    public class UserService: IUserService
    {
        private readonly ApplicationDbContext _context;
        public UserService(ApplicationDbContext context)
        {
            _context = context;

        }
        public async Task<string> AddToWishlistAsync(int userId, WishlistDto dto)
        {

            var wishlist = await _context.Wishlists.AnyAsync(p => p.UserId == userId
            && dto.ProductId == p.ProductId && dto.Size == p.Size && dto.Color == p.Color);

            if (wishlist)
                return JsonHelper.GetMessage(130);

            var wishlistItem = new Wishlist
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Size = dto.Size,
                Color = dto.Color
            };

            _context.Wishlists.Add(wishlistItem);
            await _context.SaveChangesAsync();

            return JsonHelper.GetMessage(135);

        }

        public async Task<string> AddToCartAsync(int userId, CartDto dto)
        {
            var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart 
                {
                    UserId = userId
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var existingItem = cart.Items.FirstOrDefault(i =>
                i.ProductId == dto.ProductId &&
                i.Size == dto.Size &&
                i.Color == dto.Color);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = dto.ProductId,
                    Size = dto.Size,
                    Color = dto.Color,
                    Quantity = dto.Quantity
                });
            }

            await _context.SaveChangesAsync();
            return JsonHelper.GetMessage(136);
        }

        public async Task<string> RemoveFromCartAsync(int Id, int userId)
        {
            var userCart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId); 

            if (userCart == null)
            {
                return JsonHelper.GetMessage(137);
            }

            var cartItem = await _context.CartItems.FindAsync(Id);
            if (cartItem == null)
            {
                return JsonHelper.GetMessage(138);
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return JsonHelper.GetMessage(139);
        }

        public async Task<object> GetCartByUserIdAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return JsonHelper.GetMessage(140);
            }

            return new
            {
                cart.Id,
                Items = cart.Items.Select(i => new
                {
                    i.Id,
                    ProductName = i.Product.Name,
                    i.Product.Price,
                    i.Quantity
                })
            };

        }

        public async Task<string> AddAddressAsync(AddressDto dto, int userId)
        {
            bool addressExists = await _context.Addresses.AnyAsync(a =>
                a.UserId == userId &&
                a.Street == dto.Street &&
                a.City == dto.City &&
                a.State == dto.State &&
                a.ZipCode == dto.ZipCode &&
                a.Country == dto.Country
             );

            if (addressExists)
                return null; 

            var address = new Address
            {
                UserId = userId,
                Street = dto.Street,
                City = dto.City,
                State = dto.State,
                ZipCode = dto.ZipCode,
                Country = dto.Country
            };

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            return JsonHelper.GetMessage(128);
        }
        public async Task<string> GetAddressAsync(int userId)
        {
            var addresses = await _context.Addresses
            .Where(a => a.UserId == userId)
            .ToListAsync();

            if (addresses == null || addresses.Count == 0)
                return JsonHelper.GetMessage(127);

             return JsonHelper.GetMessage(129);
        }
        public async Task<string> UpdateAddressAsync(int userId, AddressDto dto, int addressId)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

            if (address == null)
                return JsonHelper.GetMessage(127);

            var duplicate = await _context.Addresses.AnyAsync(a =>
                a.Id == addressId &&
                a.UserId == userId &&
                a.Street == dto.Street &&
                a.City == dto.City &&
                a.State == dto.State &&
                a.ZipCode == dto.ZipCode &&
                a.Country == dto.Country);

            if (duplicate)
                return JsonHelper.GetMessage(154);

            address.Street = dto.Street;
            address.City = dto.City;
            address.State = dto.State;
            address.ZipCode = dto.ZipCode;
            address.Country=dto.Country;

            await _context.SaveChangesAsync();

            return JsonHelper.GetMessage(152);
        }
        public async Task<string> DeleteAddressAsync(int userId, int addressId)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

            if (address == null)
                return JsonHelper.GetMessage(127);

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return JsonHelper.GetMessage(153);
        }
        public async Task<string> PlaceOrderAsync(int userId, OrderDto dto)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
            {
                return JsonHelper.GetMessage(133);
            }

            decimal totalAmount = 0;
            foreach (var item in cart.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                totalAmount += product.Price * item.Quantity;

                var variant = await _context.Variants.FirstOrDefaultAsync(v => v.ProductId == item.ProductId &&
                    v.Size == item.Size && v.Color == item.Color);

                 variant.Count -= item.Quantity; 
                
            }

            var order = new Order
            {
                UserId = userId,
                AddressId = dto.AddressId,
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = JsonHelper.GetMessage(142),
                OrderStatus = JsonHelper.GetMessage(143),
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Items = cart.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Size = i.Size,
                    Color = i.Color,
                    Quantity = i.Quantity

                }).ToList()
            };

            _context.Orders.Add(order);
            _context.Carts.Remove(cart);
            _context.CartItems.RemoveRange(cart.Items);
            await _context.SaveChangesAsync();

            return JsonHelper.GetMessage(141);

        }
    }
}
