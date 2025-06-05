namespace ECommerceApi.Models.Entities
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public string UserType { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}