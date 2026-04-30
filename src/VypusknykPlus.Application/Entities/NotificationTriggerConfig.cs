namespace VypusknykPlus.Application.Entities;

public class NotificationTriggerConfig
{
    public string TriggerType { get; set; } = string.Empty;
    public string? ExtraConfig { get; set; } // JSON: { "statusFilter": "any" | "Shipped" | ... }

    public bool EmailEnabled { get; set; }
    public string EmailRecipients { get; set; } = "[]"; // JSON: string[]

    public bool TelegramEnabled { get; set; }
    public string TelegramUserIds { get; set; } = "[]"; // JSON: string[]
    public bool TelegramGroupEnabled { get; set; }

    public bool SystemEnabled { get; set; }
    public string SystemAdminIds { get; set; } = "[]"; // JSON: long[]

    public string? SystemTitle { get; set; }
    public string? SystemMessage { get; set; }
    public string? EmailSubject { get; set; }
    public string? EmailMessage { get; set; }
    public string? TelegramMessage { get; set; }
}
