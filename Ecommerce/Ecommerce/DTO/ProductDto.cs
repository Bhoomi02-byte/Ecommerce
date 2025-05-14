using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTO
{
    public class ProductDto
    {
        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(10, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Dress style is required.")]
        [StringLength(50, ErrorMessage = "Dress style cannot exceed 50 characters.")]
        public string DressStyle { get; set; }

        //[Required(ErrorMessage = "Image URL is required.")]
        //[Url(ErrorMessage = "ImageUrl must be a valid URL.")]
        public string ImageUrl { get; set; }

        //[Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(10, ErrorMessage = "Category cannot exceed 50 characters.")]
        public string Category { get; set; }

        [Required]
        public List<VariantDto> Variants { get; set; }

    }
}
