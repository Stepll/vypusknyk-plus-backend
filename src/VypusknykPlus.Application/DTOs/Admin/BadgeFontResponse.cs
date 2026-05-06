namespace VypusknykPlus.Application.DTOs.Admin;

public class BadgeFontResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string FontFamily { get; set; } = string.Empty;
    public decimal PriceModifier { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}

public class SaveBadgeFontRequest
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string FontFamily { get; set; } = string.Empty;
    public decimal PriceModifier { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}
