namespace SmartGarage.Core.Interfaces;

public class NotificationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public interface INotificationService
{
    Task<List<NotificationDto>> GetByUserIdAsync(int userId);
    Task<int> GetUnreadCountAsync(int userId);
    Task SendAsync(int userId, string title, string message);
    Task MarkAsReadAsync(int notificationId);
    Task MarkAllAsReadAsync(int userId);
}
