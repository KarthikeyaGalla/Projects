namespace ECommerceApi.Models.Entities
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; }
    }
}