using System.Text.Json;

namespace VypusknykPlus.Application.DTOs.Admin;

public class AdminCertificateDesignResponse
{
    public long Id { get; set; }
    public string DesignName { get; set; } = string.Empty;
    public DateTime SavedAt { get; set; }
    public long UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string? UserEmail { get; set; }
    public JsonElement State { get; set; }
}
