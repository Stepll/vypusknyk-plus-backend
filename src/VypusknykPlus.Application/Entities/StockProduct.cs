namespace VypusknykPlus.Application.Entities;

public class StockProduct
{
    public long Id { get; set; }
    public long SubcategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool HasColor { get; set; }
    public bool HasMaterial { get; set; }
    public StockSubcategory Subcategory { get; set; } = null!;
    public ICollection<StockVariant> Variants { get; set; } = [];
}
