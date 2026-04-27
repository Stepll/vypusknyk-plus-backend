namespace VypusknykPlus.Application.Entities;

public class RibbonFont : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string FontFamily { get; set; } = string.Empty;
    public string? ImportUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;
}
