namespace ECommerceApi.Models.Entities
{
    public class ShopHour
    {
        public int ShopId { get; set; }
        public Shop Shop { get; set; }
        public string DayOfWeek { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
    }
}