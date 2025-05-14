using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTO
{
    public class WishlistDto
    {
        [Required(ErrorMessage = "ProductId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ProductId must be a positive number.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Size is required.")]
        [StringLength(5, ErrorMessage = "Size must be at most 20 characters.")]
        public string Size { get; set; }

        [Required(ErrorMessage = "Color is required.")]
        [StringLength(10, ErrorMessage = "Color must be at most 30 characters.")]
        public string Color { get; set; }

    }
}
