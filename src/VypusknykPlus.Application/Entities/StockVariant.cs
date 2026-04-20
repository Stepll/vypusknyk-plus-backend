namespace VypusknykPlus.Application.Entities;

public class StockVariant
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public string Material { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public StockProduct Product { get; set; } = null!;
    public ICollection<StockTransaction> Transactions { get; set; } = [];
}
