using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTO
{
    public class PhoneLoginDto
    {
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\+91\d{10}$", ErrorMessage = "Phone number must be in the format +91XXXXXXXXXX")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "OTP is required")]
        public string OtpCode { get; set; }

       
    }
}
