using Ecommerce.DTO;
using Ecommerce.Models.Entities;

namespace Ecommerce.Services
{
    public interface IUserService
    {
        Task<string> AddToWishlistAsync(int userId, WishlistDto dto);
        Task<string> AddToCartAsync(int userId, CartDto dto);
        Task<string> RemoveFromCartAsync(int cartItemId, int userId);
        Task<Cart> GetCartByUserIdAsync(int userId);
        Task<string> AddAddressAsync(AddressDto dto, int userId);
        Task<object> GetAddressAsync(int userId);
        Task<string> PlaceOrderAsync(int userId, OrderDto dto);
    }
}
