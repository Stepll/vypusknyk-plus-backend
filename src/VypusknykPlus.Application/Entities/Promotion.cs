namespace VypusknykPlus.Application.Entities;

public enum DiscountType { Percentage = 0, FixedAmount = 1 }
public enum PromotionScope { Global = 0, Category = 1, Volume = 2, Bundle = 3 }

public class Promotion : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public PromotionScope Scope { get; set; } = PromotionScope.Global;
    public decimal? MinOrderAmount { get; set; }
    public DateTime? StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsOneTimePerUser { get; set; }

    public ICollection<PromotionTargetCategory> TargetCategories { get; set; } = [];
    public ICollection<PromotionVolumeTier> VolumeTiers { get; set; } = [];
    public ICollection<PromotionBundleItem> BundleItems { get; set; } = [];
    public ICollection<PromotionUsage> Usages { get; set; } = [];
    public ICollection<Order> Orders { get; set; } = [];
}
