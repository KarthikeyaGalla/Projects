namespace ECommerceApi.Models.Entities
{
    public class ShopReview
    {
        public int ReviewId { get; set; }
        public int ShopId { get; set; }
        public Shop Shop { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}