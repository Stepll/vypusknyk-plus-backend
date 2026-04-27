namespace VypusknykPlus.Application.DTOs.Admin;

public class RibbonColorResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Hex { get; set; } = string.Empty;
    public string? SecondaryHex { get; set; }
    public decimal PriceModifier { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}

public class SaveRibbonColorRequest
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Hex { get; set; } = string.Empty;
    public string? SecondaryHex { get; set; }
    public decimal PriceModifier { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}
