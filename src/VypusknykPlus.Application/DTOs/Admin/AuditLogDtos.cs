namespace VypusknykPlus.Application.DTOs.Admin;

public class AuditLogResponse
{
    public long Id { get; set; }
    public long? AdminId { get; set; }
    public string AdminName { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public long EntityId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? ChangesJson { get; set; }
    public DateTime CreatedAt { get; set; }
}
