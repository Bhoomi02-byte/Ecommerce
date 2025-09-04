namespace Ecommerce.Models.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price {  get; set; }
        public string DressStyle { get; set; }
        public string ImageUrl {  get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int SellerId { get; set; }
        public User Seller { get; set; }

        public ICollection<Variant> Variants { get; set; }
        public ICollection<Wishlist> Wishlists { get; set; }
    }
}
    