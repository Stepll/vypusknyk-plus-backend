namespace VypusknykPlus.Application.Entities;

public class StockProduct
{
    public long Id { get; set; }
    public long CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public StockCategory Category { get; set; } = null!;
    public ICollection<StockVariant> Variants { get; set; } = [];
}
