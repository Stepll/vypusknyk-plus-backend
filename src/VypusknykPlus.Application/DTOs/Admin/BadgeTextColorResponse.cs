namespace VypusknykPlus.Application.DTOs.Admin;

public class BadgeTextColorResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Hex { get; set; } = string.Empty;
    public decimal PriceModifier { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}

public class SaveBadgeTextColorRequest
{
    public string Name { get; set; } = string.Empty;
    public string Hex { get; set; } = string.Empty;
    public decimal PriceModifier { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}
