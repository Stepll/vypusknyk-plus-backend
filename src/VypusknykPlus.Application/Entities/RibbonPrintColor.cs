namespace VypusknykPlus.Application.Entities;

public class RibbonPrintColor : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Hex { get; set; } = string.Empty;
    public decimal PriceModifier { get; set; } = 0;
    public bool IsForMainText { get; set; } = true;
    public bool IsForExtraText { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;
}
