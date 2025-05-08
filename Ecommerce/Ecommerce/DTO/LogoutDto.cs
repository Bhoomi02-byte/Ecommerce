using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTO
{
    public class LogoutDto
    {
        [Required]
        public string DeviceId { get; set; }

        [Required]
        public int UserId { get; set; } 
    }
}
