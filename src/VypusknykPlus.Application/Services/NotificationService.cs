using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
    private readonly string _adminPanelUrl;

    public NotificationService(AppDbContext db, INotificationPushService push, IOptions<EmailSettings> settings)
    {
        _db = db;
        _push = push;
        _adminPanelUrl = settings.Value.AdminPanelUrl.TrimEnd('/');
    }

    public async Task OnNewOrderAsync(long orderId, string orderNumber, Dictionary<string, string> context)
    {
        context["orderUrl"] = $"{_adminPanelUrl}/orders/{orderId}";
        var config = await _db.NotificationTriggerConfigs.FindAsync("new_order");
        if (config is null || !config.SystemEnabled) return;

        var adminIds = JsonSerializer.Deserialize<List<long>>(config.SystemAdminIds) ?? [];
        if (adminIds.Count == 0) return;

        var title = ApplyTemplate(config.SystemTitle, context)
            .OrDefault($"Нове замовлення #{context.GetValueOrDefault("orderNumber")}");
        var body = ApplyTemplate(config.SystemMessage, context)
            .OrDefault($"від {context.GetValueOrDefault("customerName")}");

        await DispatchAsync(adminIds, new AdminNotification
        {
            TriggerType = "new_order",
            Title = title,
            Body = body,
            EntityType = "order",
            EntityId = orderId,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        });
    }

    public async Task OnOrderStatusChangedAsync(long orderId, string orderNumber, string newStatusName, Dictionary<string, string> context)
    {
        context["orderUrl"] = $"{_adminPanelUrl}/orders/{orderId}";
        await DispatchStatusTriggerAsync("order_status_changed", orderId, context);
        await DispatchStatusTriggerAsync($"order_status_changed:{newStatusName}", orderId, context);
    }

    private async Task DispatchStatusTriggerAsync(string triggerType, long orderId, Dictionary<string, string> context)
    {
        var config = await _db.NotificationTriggerConfigs.FindAsync(triggerType);
        if (config is null || !config.SystemEnabled) return;

        var adminIds = JsonSerializer.Deserialize<List<long>>(config.SystemAdminIds) ?? [];
        if (adminIds.Count == 0) return;

        var orderNumber = context.GetValueOrDefault("orderNumber", "");
        var statusName = context.GetValueOrDefault("statusName", "");

        var title = ApplyTemplate(config.SystemTitle, context)
            .OrDefault("Статус замовлення змінено");
        var body = ApplyTemplate(config.SystemMessage, context)
            .OrDefault($"Замовлення #{orderNumber} → {statusName}");

        await DispatchAsync(adminIds, new AdminNotification
        {
            TriggerType = triggerType,
            Title = title,
            Body = body,
            EntityType = "order",
            EntityId = orderId,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        });
    }

    public async Task OnNewUserAsync(long userId, Dictionary<string, string> context)
    {
        context["userUrl"] = $"{_adminPanelUrl}/users/{userId}";
        var config = await _db.NotificationTriggerConfigs.FindAsync("new_user");
        if (config is null || !config.SystemEnabled) return;

        var adminIds = JsonSerializer.Deserialize<List<long>>(config.SystemAdminIds) ?? [];
        if (adminIds.Count == 0) return;

        var fullName = context.GetValueOrDefault("fullName", "");
        var email = context.GetValueOrDefault("email", "");

        var title = ApplyTemplate(config.SystemTitle, context)
            .OrDefault("Нова реєстрація");
        var body = ApplyTemplate(config.SystemMessage, context)
            .OrDefault(string.IsNullOrEmpty(email) ? fullName : $"{fullName} ({email})");

        await DispatchAsync(adminIds, new AdminNotification
        {
            TriggerType = "new_user",
            Title = title,
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

        var result = AllTriggerTypes.Select(type =>
            configMap.TryGetValue(type, out var c)
                ? MapConfig(c)
                : new NotificationTriggerConfigResponse
                {
                    TriggerType = type,
                    DisplayName = TriggerDisplayNames.GetValueOrDefault(type, type)
                }
        ).ToList();

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

        config.EmailEnabled = request.EmailEnabled;
        config.EmailRecipients = JsonSerializer.Serialize(request.EmailRecipients);
        config.EmailSubject = request.EmailSubject;
        config.EmailMessage = request.EmailMessage;
        config.TelegramEnabled = request.TelegramEnabled;
        config.TelegramUserIds = JsonSerializer.Serialize(request.TelegramUserIds);
        config.TelegramGroupEnabled = request.TelegramGroupEnabled;
        config.TelegramMessage = request.TelegramMessage;
        config.SystemEnabled = request.SystemEnabled;
        config.SystemAdminIds = JsonSerializer.Serialize(request.SystemAdminIds);
        config.SystemTitle = request.SystemTitle;
        config.SystemMessage = request.SystemMessage;

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

    private static string ApplyTemplate(string? template, Dictionary<string, string> vars)
    {
        if (string.IsNullOrEmpty(template)) return string.Empty;
        foreach (var (key, value) in vars)
            template = template.Replace($"{{{{{key}}}}}", value);
        return template;
    }

    private static NotificationTriggerConfigResponse MapConfig(NotificationTriggerConfig c) => new()
    {
        TriggerType = c.TriggerType,
        DisplayName = TriggerDisplayNames.GetValueOrDefault(c.TriggerType, c.TriggerType),
        EmailEnabled = c.EmailEnabled,
        EmailRecipients = JsonSerializer.Deserialize<List<string>>(c.EmailRecipients) ?? [],
        EmailSubject = c.EmailSubject,
        EmailMessage = c.EmailMessage,
        TelegramEnabled = c.TelegramEnabled,
        TelegramUserIds = JsonSerializer.Deserialize<List<string>>(c.TelegramUserIds) ?? [],
        TelegramGroupEnabled = c.TelegramGroupEnabled,
        TelegramMessage = c.TelegramMessage,
        SystemEnabled = c.SystemEnabled,
        SystemAdminIds = JsonSerializer.Deserialize<List<long>>(c.SystemAdminIds) ?? [],
        SystemTitle = c.SystemTitle,
        SystemMessage = c.SystemMessage
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

internal static class StringExtensions
{
    public static string OrDefault(this string value, string fallback)
        => string.IsNullOrWhiteSpace(value) ? fallback : value;
}
