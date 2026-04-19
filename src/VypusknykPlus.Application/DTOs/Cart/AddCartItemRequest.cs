using VypusknykPlus.Application.ValueObjects;

namespace VypusknykPlus.Application.DTOs.Cart;

public class AddCartItemRequest
{
    public long? ProductId { get; set; }
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public int Qty { get; set; }
    public NamesData? NamesData { get; set; }
    public RibbonCustomization? RibbonCustomization { get; set; }
}
