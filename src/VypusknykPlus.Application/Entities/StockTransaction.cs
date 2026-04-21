namespace VypusknykPlus.Application.Entities;

public class StockTransaction
{
    public long Id { get; set; }
    public long VariantId { get; set; }
    public long? DeliveryItemId { get; set; }
    public string Type { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime Date { get; set; }
    public string Note { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public long? OrderId { get; set; }
    public StockVariant Variant { get; set; } = null!;
    public DeliveryItem? DeliveryItem { get; set; }
    public Order? Order { get; set; }
}
