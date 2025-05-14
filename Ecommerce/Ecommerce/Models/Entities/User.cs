using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Models.Entities
{
    public enum UserRole{
        Seller,
        Customer
    }
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string PhoneNumber { get; set; }
        public UserRole Role { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
        public ICollection<Wishlist> Wishlists { get; set; }

    }
}
