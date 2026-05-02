namespace VypusknykPlus.Application.Entities;

public class PromoCode : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string CardColor { get; set; } = "#FF6B9D";
    public string? Description { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public int? MaxUsages { get; set; }
    public int UsagesCount { get; set; }
    public bool IsOneTimePerUser { get; set; }
    public DateTime? StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<UserPromoCodeCard> Cards { get; set; } = [];
    public ICollection<PromoCodeUsage> Usages { get; set; } = [];
    public ICollection<Order> Orders { get; set; } = [];
}
