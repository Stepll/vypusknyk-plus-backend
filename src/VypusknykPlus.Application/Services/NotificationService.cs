using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Notifications;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Services;

public class NotificationService : INotificationService
{
    private static readonly Dictionary<string, string> TriggerDisplayNames = new()
    {
        ["new_order"] = "Нове замовлення",
        ["order_status_changed"] = "Замовлення змінило статус",
        ["new_user"] = "Нова реєстрація користувача"
    };

    private static readonly string[] AllTriggerTypes = ["new_order", "order_status_changed", "new_user"];

    private readonly AppDbContext _db;
    private readonly INotificationPushService _push;

    public NotificationService(AppDbContext db, INotificationPushService push)
    {
        _db = db;
        _push = push;
    }

    public async Task OnNewOrderAsync(long orderId, string orderNumber, string customerName)
    {
        var config = await _db.NotificationTriggerConfigs.FindAsync("new_order");
        if (config is null || !config.SystemEnabled) return;

        var adminIds = JsonSerializer.Deserialize<List<long>>(config.SystemAdminIds) ?? [];
        if (adminIds.Count == 0) return;

        await DispatchAsync(adminIds, new AdminNotification
        {
            TriggerType = "new_order",
            Title = "Нове замовлення",
            Body = $"Замовлення #{orderNumber} від {customerName}",
            EntityType = "order",
            EntityId = orderId,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        });
    }

    public async Task OnOrderStatusChangedAsync(long orderId, string orderNumber, string newStatusName)
    {
        // Catch-all: fires for every status change
        await DispatchStatusTriggerAsync("order_status_changed", orderId, orderNumber, newStatusName);
        // Specific: fires only for this status
        await DispatchStatusTriggerAsync($"order_status_changed:{newStatusName}", orderId, orderNumber, newStatusName);
    }

    private async Task DispatchStatusTriggerAsync(string triggerType, long orderId, string orderNumber, string newStatusName)
    {
        var config = await _db.NotificationTriggerConfigs.FindAsync(triggerType);
        if (config is null || !config.SystemEnabled) return;

        var adminIds = JsonSerializer.Deserialize<List<long>>(config.SystemAdminIds) ?? [];
        if (adminIds.Count == 0) return;

        await DispatchAsync(adminIds, new AdminNotification
        {
            TriggerType = triggerType,
            Title = "Статус замовлення змінено",
            Body = $"Замовлення #{orderNumber} → {newStatusName}",
            EntityType = "order",
            EntityId = orderId,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        });
    }

    public async Task OnNewUserAsync(long userId, string fullName, string? email)
    {
        var config = await _db.NotificationTriggerConfigs.FindAsync("new_user");
        if (config is null || !config.SystemEnabled) return;

        var adminIds = JsonSerializer.Deserialize<List<long>>(config.SystemAdminIds) ?? [];
        if (adminIds.Count == 0) return;

        var body = email is not null ? $"{fullName} ({email})" : fullName;

        await DispatchAsync(adminIds, new AdminNotification
        {
            TriggerType = "new_user",
            Title = "Нова реєстрація",
            Body = body,
            EntityType = "user",
            EntityId = userId,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        });
    }

    public async Task<List<AdminNotificationDto>> GetMyNotificationsAsync(long adminId, int limit = 50)
    {
        return await _db.AdminNotifications
            .Where(n => n.AdminId == adminId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(limit)
            .Select(n => ToDto(n))
            .ToListAsync();
    }

    public async Task MarkReadAsync(long notificationId, long adminId)
    {
        var n = await _db.AdminNotifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.AdminId == adminId);
        if (n is null) return;
        n.IsRead = true;
        await _db.SaveChangesAsync();
    }

    public async Task MarkAllReadAsync(long adminId)
    {
        await _db.AdminNotifications
            .Where(n => n.AdminId == adminId && !n.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
    }

    public async Task<int> GetUnreadCountAsync(long adminId)
    {
        return await _db.AdminNotifications
            .CountAsync(n => n.AdminId == adminId && !n.IsRead);
    }

    public async Task<List<NotificationTriggerConfigResponse>> GetTriggerConfigsAsync()
    {
        var configs = await _db.NotificationTriggerConfigs.ToListAsync();
        var configMap = configs.ToDictionary(c => c.TriggerType);

        // Base triggers (always present in response)
        var result = AllTriggerTypes.Select(type =>
            configMap.TryGetValue(type, out var c)
                ? MapConfig(c)
                : new NotificationTriggerConfigResponse
                {
                    TriggerType = type,
                    DisplayName = TriggerDisplayNames.GetValueOrDefault(type, type)
                }
        ).ToList();

        // Sub-triggers (e.g. "order_status_changed:Прийнято")
        result.AddRange(configs
            .Where(c => c.TriggerType.Contains(':'))
            .Select(MapConfig));

        return result;
    }

    public async Task UpdateTriggerConfigAsync(string triggerType, UpdateNotificationTriggerConfigRequest request)
    {
        var config = await _db.NotificationTriggerConfigs.FindAsync(triggerType);
        if (config is null)
        {
            config = new NotificationTriggerConfig { TriggerType = triggerType };
            _db.NotificationTriggerConfigs.Add(config);
        }

        config.ExtraConfig = request.ExtraConfig;
        config.EmailEnabled = request.EmailEnabled;
        config.EmailRecipients = JsonSerializer.Serialize(request.EmailRecipients);
        config.TelegramEnabled = request.TelegramEnabled;
        config.TelegramUserIds = JsonSerializer.Serialize(request.TelegramUserIds);
        config.TelegramGroupEnabled = request.TelegramGroupEnabled;
        config.SystemEnabled = request.SystemEnabled;
        config.SystemAdminIds = JsonSerializer.Serialize(request.SystemAdminIds);

        await _db.SaveChangesAsync();
    }

    private async Task DispatchAsync(List<long> adminIds, AdminNotification template)
    {
        var notifications = adminIds.Select(adminId => new AdminNotification
        {
            AdminId = adminId,
            TriggerType = template.TriggerType,
            Title = template.Title,
            Body = template.Body,
            EntityType = template.EntityType,
            EntityId = template.EntityId,
            IsRead = false,
            CreatedAt = template.CreatedAt
        }).ToList();

        _db.AdminNotifications.AddRange(notifications);
        await _db.SaveChangesAsync();

        foreach (var n in notifications)
            await _push.PushToAdminAsync(n.AdminId, ToDto(n));
    }

    private static NotificationTriggerConfigResponse MapConfig(NotificationTriggerConfig c) => new()
    {
        TriggerType = c.TriggerType,
        DisplayName = TriggerDisplayNames.GetValueOrDefault(c.TriggerType, c.TriggerType),
        ExtraConfig = c.ExtraConfig,
        EmailEnabled = c.EmailEnabled,
        EmailRecipients = JsonSerializer.Deserialize<List<string>>(c.EmailRecipients) ?? [],
        TelegramEnabled = c.TelegramEnabled,
        TelegramUserIds = JsonSerializer.Deserialize<List<string>>(c.TelegramUserIds) ?? [],
        TelegramGroupEnabled = c.TelegramGroupEnabled,
        SystemEnabled = c.SystemEnabled,
        SystemAdminIds = JsonSerializer.Deserialize<List<long>>(c.SystemAdminIds) ?? []
    };

    private static AdminNotificationDto ToDto(AdminNotification n) => new()
    {
        Id = n.Id,
        TriggerType = n.TriggerType,
        Title = n.Title,
        Body = n.Body,
        EntityType = n.EntityType,
        EntityId = n.EntityId,
        IsRead = n.IsRead,
        CreatedAt = n.CreatedAt
    };
}
