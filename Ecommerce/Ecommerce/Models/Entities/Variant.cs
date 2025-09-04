namespace Ecommerce.Models.Entities
{
    public class Variant
    {
        public int Id { get; set; }
        public int Count {  get; set; }
        public string Size {  get; set; }
        public string Color { get; set; }
        public int ProductId {  get; set; }
        public Product Product { get; set; }
    }
}
