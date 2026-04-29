namespace VypusknykPlus.Application.Entities;

public class AdminNotification
{
    public long Id { get; set; }
    public long AdminId { get; set; }
    public Admin Admin { get; set; } = null!;
    public string TriggerType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public long? EntityId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
