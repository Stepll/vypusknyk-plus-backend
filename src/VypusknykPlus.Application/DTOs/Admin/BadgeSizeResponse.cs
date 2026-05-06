namespace VypusknykPlus.Application.DTOs.Admin;

public class BadgeSizeResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Diameter { get; set; }
    public decimal PriceModifier { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}

public class SaveBadgeSizeRequest
{
    public string Name { get; set; } = string.Empty;
    public int Diameter { get; set; }
    public decimal PriceModifier { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}
