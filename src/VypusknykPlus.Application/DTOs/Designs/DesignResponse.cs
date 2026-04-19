using VypusknykPlus.Application.ValueObjects;

namespace VypusknykPlus.Application.DTOs.Designs;

public class DesignResponse
{
    public long Id { get; set; }
    public string DesignName { get; set; } = string.Empty;
    public DateTime SavedAt { get; set; }
    public RibbonState State { get; set; } = new();
}
