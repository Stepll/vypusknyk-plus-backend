using VypusknykPlus.Application.ValueObjects;

namespace VypusknykPlus.Application.Entities;

public class SavedDesign : BaseEntity
{
    public string DesignName { get; set; } = string.Empty;
    public DateTime SavedAt { get; set; }
    public RibbonState State { get; set; } = new();

    public long UserId { get; set; }
    public User User { get; set; } = null!;
}
