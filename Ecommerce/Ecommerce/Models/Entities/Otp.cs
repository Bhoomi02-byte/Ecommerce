namespace Ecommerce.Models.Entities
{
    public class Otp
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string OtpCode { get; set; }
        public DateTime ExpiryTime { get; set; }
      
    }
}
