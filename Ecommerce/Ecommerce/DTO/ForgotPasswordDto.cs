using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTO
{
    public class ForgotPasswordDto
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
