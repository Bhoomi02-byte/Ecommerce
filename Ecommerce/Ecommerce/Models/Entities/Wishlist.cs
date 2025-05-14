namespace Ecommerce.Models.Entities
{
    public class Wishlist
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }

        public User User { get; set; }
        public Product Product { get; set; }

    }
}
