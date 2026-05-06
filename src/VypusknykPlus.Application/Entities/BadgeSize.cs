namespace VypusknykPlus.Application.Entities;

public class BadgeSize : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int Diameter { get; set; }
    public decimal PriceModifier { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;
}
