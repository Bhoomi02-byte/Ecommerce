using System.Buffers.Text;
using System.ComponentModel.DataAnnotations;
using Ecommerce.Models.Entities;
using Microsoft.SqlServer.Server;

namespace Ecommerce.DTO
{
    public class EmailSignupDto
    {
       

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }
 
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\+91\d{10}$", ErrorMessage = "Phone number must be in the format +91XXXXXXXXXX")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "User role is required")]
        [EnumDataType(typeof(UserRole), ErrorMessage = "Invalid user role")]
        public UserRole Role { get; set; }
       

    }
}
