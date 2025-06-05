namespace ECommerceApi.Models.Entities
{
    public class SavedLocation
    {
        public int LocationId { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string Label { get; set; }
        public string Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}