namespace VypusknykPlus.Application.Entities;

public class StockSubcategory
{
    public long Id { get; set; }
    public long CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public StockCategory Category { get; set; } = null!;
    public ICollection<StockProduct> Products { get; set; } = [];
}
