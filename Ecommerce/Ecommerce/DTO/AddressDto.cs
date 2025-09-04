using System.ComponentModel.DataAnnotations;
using Microsoft.OpenApi.MicrosoftExtensions;

namespace Ecommerce.DTO
{
    public class AddressDto
    {
        [Required(ErrorMessage="Street is required.")]
        [StringLength(100, ErrorMessage ="Street can't be longer 100 characters")]
        public string Street { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(50, ErrorMessage = "City can't be longer than 50 characters.")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required.")]
        [StringLength(50, ErrorMessage = "State can't be longer than 50 characters.")]
        public string State { get; set; }

        [Required(ErrorMessage = "Zip code is required.")]
        //[RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid Zip code format.")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        [StringLength(50, ErrorMessage = "Country can't be longer than 50 characters.")]
        public string Country { get; set; }
    }
}
