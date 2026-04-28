using VypusknykPlus.Application.ValueObjects;

namespace VypusknykPlus.Application.DTOs.Admin;

public class AdminSavedDesignDetailResponse
{
    public long Id { get; set; }
    public string DesignName { get; set; } = string.Empty;
    public DateTime SavedAt { get; set; }
    public long UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string? UserEmail { get; set; }
    public RibbonState State { get; set; } = new();
}
