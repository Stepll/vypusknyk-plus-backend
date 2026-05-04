namespace VypusknykPlus.Application.Entities;

public class PromotionTargetCategory
{
    public long Id { get; set; }
    public long PromotionId { get; set; }
    public Promotion Promotion { get; set; } = null!;
    public long? CategoryId { get; set; }
    public ProductCategory? Category { get; set; }
    public long? SubcategoryId { get; set; }
    public ProductSubcategory? Subcategory { get; set; }
}
