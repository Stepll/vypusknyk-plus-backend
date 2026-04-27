namespace VypusknykPlus.Application.Entities;

public class RibbonEmblem : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? SvgKeyLeft { get; set; }
    public string? SvgKeyRight { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;
}
