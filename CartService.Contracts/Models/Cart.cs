using LiteDB;

namespace CartService.Contracts.Models
{
    public class Cart
    {
        [BsonId]
        public string Id { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
    }

    public class CartItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "TestItem";
        public ImageInfo? Image { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = "UAH";
        public int Quantity { get; set; } = 1;
    }

    public class ImageInfo
    {
        public string? Url { get; set; }
        public string? Alt { get; set; }
    }
}