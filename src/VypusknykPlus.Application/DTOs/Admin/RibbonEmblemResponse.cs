namespace VypusknykPlus.Application.DTOs.Admin;

public class RibbonEmblemResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? SvgUrl { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}

public class SaveRibbonEmblemRequest
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}
