using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTO
{
    public class PhoneOtpRequestDto
    {
      
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [RegularExpression(@"^\+91\d{10}$", ErrorMessage = "Phone number must be in the format +91XXXXXXXXXX")]
        public string PhoneNumber { get; set; }
    }
}
