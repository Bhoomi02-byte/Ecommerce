namespace Ecommerce.Models.Entities
{
    public class PasswordResetToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public int UserId { get; set; }
        public DateTime ExpiryTime { get; set; }
        public User User { get; set; }
    }
}
