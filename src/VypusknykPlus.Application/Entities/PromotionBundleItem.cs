namespace VypusknykPlus.Application.Entities;

public class PromotionBundleItem
{
    public long Id { get; set; }
    public long PromotionId { get; set; }
    public Promotion Promotion { get; set; } = null!;
    public long SubcategoryId { get; set; }
    public ProductSubcategory Subcategory { get; set; } = null!;
    public int RequiredQty { get; set; }
}
