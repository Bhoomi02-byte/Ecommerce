using System.Net;

namespace Ecommerce.Models.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AddressId { get; set; }
        public string PaymentMethod { get; set; } // ONLINE, COD
        public string PaymentStatus { get; set; } // PENDING, SUCCESS, CANCELLED
        public string OrderStatus { get; set; } = "PLACED";
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public Address Address { get; set; }
    }
}
