using System.ComponentModel.DataAnnotations;
using Ecommerce.Models.Entities;

namespace Ecommerce.DTO
{
    public class PhoneSignupDto
    {


        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name must be between 2 and 50 characters", MinimumLength = 2)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\+91\d{10}$", ErrorMessage = "Phone number must be in the format +91XXXXXXXXXX")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "OTP is required")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be a 6-digit number")]
        public string Otp { get; set; }
 
        [Required(ErrorMessage = "Role is required")]
        [EnumDataType(typeof(UserRole), ErrorMessage = "Invalid role value")]
        public UserRole Role { get; set; }
    }
}
