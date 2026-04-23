using VypusknykPlus.Application.ValueObjects;

namespace VypusknykPlus.Application.Entities;

public class OrderItem : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public NamesData? NamesData { get; set; }
    public RibbonCustomization? RibbonCustomization { get; set; }

    public string? ProductCategory { get; set; }

    public long OrderId { get; set; }
    public Order Order { get; set; } = null!;
}
