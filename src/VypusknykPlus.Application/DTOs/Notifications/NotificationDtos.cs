namespace VypusknykPlus.Application.DTOs.Notifications;

public class NotificationTriggerConfigResponse
{
    public string TriggerType { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? ExtraConfig { get; set; }

    public bool EmailEnabled { get; set; }
    public List<string> EmailRecipients { get; set; } = [];

    public bool TelegramEnabled { get; set; }
    public List<string> TelegramUserIds { get; set; } = [];
    public bool TelegramGroupEnabled { get; set; }

    public bool SystemEnabled { get; set; }
    public List<long> SystemAdminIds { get; set; } = [];
}

public class UpdateNotificationTriggerConfigRequest
{
    public string? ExtraConfig { get; set; }

    public bool EmailEnabled { get; set; }
    public List<string> EmailRecipients { get; set; } = [];

    public bool TelegramEnabled { get; set; }
    public List<string> TelegramUserIds { get; set; } = [];
    public bool TelegramGroupEnabled { get; set; }

    public bool SystemEnabled { get; set; }
    public List<long> SystemAdminIds { get; set; } = [];
}

public class AdminNotificationDto
{
    public long Id { get; set; }
    public string TriggerType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public long? EntityId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
