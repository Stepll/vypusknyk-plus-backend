namespace VypusknykPlus.Application.Entities;

public class Delivery
{
    public long Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public long? SupplierId { get; set; }
    public DateTime ExpectedDate { get; set; }
    public string Status { get; set; } = "pending"; // pending | partial | received
    public string? Note { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public Supplier? Supplier { get; set; }
    public ICollection<DeliveryItem> Items { get; set; } = [];
}
