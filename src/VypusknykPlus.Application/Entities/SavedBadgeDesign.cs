namespace VypusknykPlus.Application.Entities;

public class SavedBadgeDesign : BaseEntity
{
    public string DesignName { get; set; } = string.Empty;
    public DateTime SavedAt { get; set; }
    public string StateJson { get; set; } = "{}";
    public long UserId { get; set; }
    public User User { get; set; } = null!;
}
