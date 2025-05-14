using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTO
{
    public class VariantDto
    {
        [Required(ErrorMessage = "Size is required.")]
        [StringLength(5, ErrorMessage = "Size must be at most 20 characters.")]
        public string Size { get; set; }

        [Required]
        public List<ColorOptionDto> Options { get; set; }
    }
}
