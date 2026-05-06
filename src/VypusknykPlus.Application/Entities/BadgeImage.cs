namespace VypusknykPlus.Application.Entities;

public class BadgeImage : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? ImageKey { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;
}
