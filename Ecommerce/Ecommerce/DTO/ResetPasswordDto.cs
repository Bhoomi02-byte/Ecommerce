using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTO
{
    public class ResetPasswordDto
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
