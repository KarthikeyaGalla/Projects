namespace ECommerceApi.Models.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public int ShopId { get; set; }
        public Shop Shop { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CategoryId { get; set; }
        public ProductCategory Category { get; set; }
        public ICollection<ProductTag> ProductTags { get; set; }
        public ICollection<ProductReview> Reviews { get; set; }
    }
}