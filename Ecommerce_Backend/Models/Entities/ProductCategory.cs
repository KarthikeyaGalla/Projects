namespace ECommerceApi.Models.Entities
{
    public class ProductCategory
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}