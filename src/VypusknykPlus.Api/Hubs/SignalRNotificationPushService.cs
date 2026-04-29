using Microsoft.AspNetCore.SignalR;
using VypusknykPlus.Application.DTOs.Notifications;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Hubs;

public class SignalRNotificationPushService : INotificationPushService
{
    private readonly IHubContext<ChatHub> _hub;

    public SignalRNotificationPushService(IHubContext<ChatHub> hub) => _hub = hub;

    public Task PushToAdminAsync(long adminId, AdminNotificationDto notification)
        => _hub.Clients.Group($"admin:{adminId}").SendAsync("ReceiveAdminNotification", notification);
}
