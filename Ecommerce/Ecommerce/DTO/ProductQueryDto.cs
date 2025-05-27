namespace Ecommerce.DTO
{
    public class ProductQueryDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Search {  get; set; }
        public string? Category {  get; set; }

    }
}
