namespace Ecommerce.Models.Entities
{
   
        public class RefreshToken
        {
            public int Id { get; set; } 
            public string Token { get; set; } 
            public int UserId { get; set; }
            public User User { get; set; } 
            public DateTime ExpiryDate { get; set; }
            //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public string DeviceId { get; set; } = Guid.NewGuid().ToString();
    }

    
}
