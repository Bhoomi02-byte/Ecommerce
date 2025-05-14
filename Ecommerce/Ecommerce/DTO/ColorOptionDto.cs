using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTO
{
    public class ColorOptionDto
    {

        [Required(ErrorMessage = "Color is required.")]
        [StringLength(10, ErrorMessage = "Color must be at most 30 characters.")]
        public string Color { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 10000.")]
        public int Count { get; set; }

    }
}
