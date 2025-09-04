using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTO
{
    public class LogoutDto
    {
        [Required(ErrorMessage = "DeviceId is required.")]
        //[ValidGuid(ErrorMessage = "DeviceId must be a valid non-empty GUID.")]
        public string DeviceId { get; set; }

        [Required(ErrorMessage = "UserId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "UserId must be a positive integer.")]
        public int UserId { get; set; }
    }
}
