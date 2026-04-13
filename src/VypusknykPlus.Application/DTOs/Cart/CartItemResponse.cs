using VypusknykPlus.Application.ValueObjects;

namespace VypusknykPlus.Application.DTOs.Cart;

public class CartItemResponse
{
    public Guid Id { get; set; }
    public int? ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductCategory { get; set; } = string.Empty;
    public string? ProductColor { get; set; }
    public decimal BasePrice { get; set; }
    public int Qty { get; set; }
    public NamesData? NamesData { get; set; }
    public RibbonCustomization? RibbonCustomization { get; set; }
}
