namespace ECommerceApi.Models.DTOs
{
    public class ShopHourDto
    {
        public int ShopId { get; set; }
        public string DayOfWeek { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
    }
}