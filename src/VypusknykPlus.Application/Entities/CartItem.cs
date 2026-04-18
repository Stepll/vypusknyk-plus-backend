using VypusknykPlus.Application.ValueObjects;

namespace VypusknykPlus.Application.Entities;

public class CartItem : BaseEntity
{
    public int Qty { get; set; }

    public int? ProductId { get; set; }
    public Product? Product { get; set; }

    public ProductSnapshot? ProductSnapshot { get; set; }
    public NamesData? NamesData { get; set; }
    public RibbonCustomization? RibbonCustomization { get; set; }

    public long UserId { get; set; }
    public User User { get; set; } = null!;
}
