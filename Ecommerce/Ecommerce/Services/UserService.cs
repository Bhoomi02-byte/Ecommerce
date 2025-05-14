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
                return  "Item already in wishlist.";

            var wishlistItem = new Wishlist
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Size = dto.Size,
                Color = dto.Color
            };

            _context.Wishlists.Add(wishlistItem);
            await _context.SaveChangesAsync();

            return "Added to wishlist";

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
            return "Item added to cart successfully";
        }

        public async Task<string> RemoveFromCartAsync(int cartItemId, int userId)
        {
            var userCart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId); 

            if (userCart == null)
            {
                return "User cart not found.";
            }

            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
            {
                return "Cart item not found.";
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return "Item removed from cart.";
        }

        public async Task<Cart> GetCartByUserIdAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            return cart;
        }

        public async Task<string> AddAddressAsync(AddressDto dto, int userId)
        {
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

            return "Address added successfully.";
        }
        public async Task<object> GetAddressAsync( int userId)
        {
            var addresses = await _context.Addresses
            .Where(a => a.UserId == userId)
            .ToListAsync();

             return addresses;
        }
        public async Task<string> PlaceOrderAsync(int userId, OrderDto dto)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
            {
                return "Cart is empty.";
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
                PaymentStatus = "PENDING",
                OrderStatus = "PLACED",
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
            _context.CartItems.RemoveRange(cart.Items);
            await _context.SaveChangesAsync();

            return "Order placed successfully.";
        }
    }
}
