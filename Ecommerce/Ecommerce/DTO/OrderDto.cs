using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTO
{
    public class OrderDto
    {
        [Required(ErrorMessage = "AddressId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "AddressId must be a positive integer.")]
        public int AddressId { get; set; }

        [Required(ErrorMessage = "PaymentMethod is required.")]
        [StringLength(10, ErrorMessage = "PaymentMethod cannot exceed 50 characters.")]
        public string PaymentMethod { get; set; }
    }
}
