using VypusknykPlus.Application.DTOs.Notifications;

namespace VypusknykPlus.Application.Services;

public interface INotificationPushService
{
    Task PushToAdminAsync(long adminId, AdminNotificationDto notification);
}
