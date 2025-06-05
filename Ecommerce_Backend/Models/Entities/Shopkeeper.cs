namespace ECommerceApi.Models.Entities
{
    public class Shopkeeper
    {
        public int ShopkeeperId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Phone { get; set; }
        public DateTime JoinedDate { get; set; }
        public ICollection<Shop> Shops { get; set; }
    }
}