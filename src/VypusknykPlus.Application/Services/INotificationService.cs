namespace VypusknykPlus.Application.Services;

public interface INotificationService
{
    Task OnNewOrderAsync(long orderId, string orderNumber, string customerName);
    Task OnOrderStatusChangedAsync(long orderId, string orderNumber, string newStatusName);
    Task OnNewUserAsync(long userId, string fullName, string? email);
    Task<List<DTOs.Notifications.AdminNotificationDto>> GetMyNotificationsAsync(long adminId, int limit = 50);
    Task MarkReadAsync(long notificationId, long adminId);
    Task MarkAllReadAsync(long adminId);
    Task<int> GetUnreadCountAsync(long adminId);
    Task<List<DTOs.Notifications.NotificationTriggerConfigResponse>> GetTriggerConfigsAsync();
    Task UpdateTriggerConfigAsync(string triggerType, DTOs.Notifications.UpdateNotificationTriggerConfigRequest request);
}
