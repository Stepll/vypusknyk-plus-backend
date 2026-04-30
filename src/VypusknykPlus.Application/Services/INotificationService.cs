namespace VypusknykPlus.Application.Services;

public interface INotificationService
{
    Task OnNewOrderAsync(long orderId, string orderNumber, Dictionary<string, string> context);
    Task OnOrderStatusChangedAsync(long orderId, string orderNumber, string newStatusName, Dictionary<string, string> context);
    Task OnNewUserAsync(long userId, Dictionary<string, string> context);
    Task<List<DTOs.Notifications.AdminNotificationDto>> GetMyNotificationsAsync(long adminId, int limit = 50);
    Task MarkReadAsync(long notificationId, long adminId);
    Task MarkAllReadAsync(long adminId);
    Task<int> GetUnreadCountAsync(long adminId);
    Task<List<DTOs.Notifications.NotificationTriggerConfigResponse>> GetTriggerConfigsAsync();
    Task UpdateTriggerConfigAsync(string triggerType, DTOs.Notifications.UpdateNotificationTriggerConfigRequest request);
}
