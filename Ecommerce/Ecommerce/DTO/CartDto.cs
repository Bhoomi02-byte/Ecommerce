using System.ComponentModel.DataAnnotations;
using Ecommerce.Models.Entities;

namespace Ecommerce.DTO
{
    public class CartDto
    {
        [Required(ErrorMessage = "ProductId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ProductId must be a positive number.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Size is required.")]
        [StringLength(5, ErrorMessage = "Size can't be longer than 20 characters.")]
        public string Size { get; set; }

        [Required(ErrorMessage = "Color is required.")]
        [StringLength(10, ErrorMessage = "Color can't be longer than 30 characters.")]
        public string Color { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 1000.")]
        public int Quantity { get; set; }

    }
}
