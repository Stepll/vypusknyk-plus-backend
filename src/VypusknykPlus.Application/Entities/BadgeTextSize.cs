namespace VypusknykPlus.Application.Entities;

public class BadgeTextSize : BaseEntity
{
    public string Label { get; set; } = string.Empty;
    public int Value { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;
}
