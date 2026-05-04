namespace VypusknykPlus.Application.Entities;

public class PromotionVolumeTier
{
    public long Id { get; set; }
    public long PromotionId { get; set; }
    public Promotion Promotion { get; set; } = null!;
    public int MinQty { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
}
