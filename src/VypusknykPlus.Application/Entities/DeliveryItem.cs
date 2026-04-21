namespace VypusknykPlus.Application.Entities;

public class DeliveryItem
{
    public long Id { get; set; }
    public long DeliveryId { get; set; }
    public long ProductId { get; set; }
    public string Material { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int ExpectedQty { get; set; }
    public int ReceivedQty { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public Delivery Delivery { get; set; } = null!;
    public StockProduct Product { get; set; } = null!;
    public ICollection<StockTransaction> Transactions { get; set; } = [];
}
