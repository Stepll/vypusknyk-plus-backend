namespace VypusknykPlus.Application.Entities;

public class StockCategory
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public ICollection<StockSubcategory> Subcategories { get; set; } = [];
}
