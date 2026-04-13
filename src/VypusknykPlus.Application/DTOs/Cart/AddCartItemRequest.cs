using VypusknykPlus.Application.ValueObjects;

namespace VypusknykPlus.Application.DTOs.Cart;

public class AddCartItemRequest
{
    public int ProductId { get; set; }
    public int Qty { get; set; }
    public NamesData? NamesData { get; set; }
    public RibbonCustomization? RibbonCustomization { get; set; }
}
