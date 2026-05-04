namespace VypusknykPlus.Application.DTOs.Promotions;

// ─── Shared sub-DTOs ───────────────────────────────────────────────────────────

public class PromotionTargetDto
{
    public long? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public long? SubcategoryId { get; set; }
    public string? SubcategoryName { get; set; }
}

public class VolumeTierDto
{
    public long Id { get; set; }
    public int MinQty { get; set; }
    public string DiscountType { get; set; } = "Percentage";
    public decimal DiscountValue { get; set; }
}

public class BundleItemDto
{
    public long Id { get; set; }
    public long SubcategoryId { get; set; }
    public string SubcategoryName { get; set; } = string.Empty;
    public int RequiredQty { get; set; }
}

// ─── Admin responses ───────────────────────────────────────────────────────────

public class AdminPromotionResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DiscountType { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public string Scope { get; set; } = string.Empty;
    public List<PromotionTargetDto> Targets { get; set; } = [];
    public List<VolumeTierDto> VolumeTiers { get; set; } = [];
    public List<BundleItemDto> BundleItems { get; set; } = [];
    public decimal? MinOrderAmount { get; set; }
    public DateTime? StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsOneTimePerUser { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class AdminPromoCodeResponse
{
    public long Id { get; set; }
    public string? Code { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string CardColor { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DiscountType { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public int? MaxUsages { get; set; }
    public int UsagesCount { get; set; }
    public bool IsOneTimePerUser { get; set; }
    public DateTime? StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}

// ─── Admin save requests ───────────────────────────────────────────────────────

public class SaveVolumeTierRequest
{
    public int MinQty { get; set; }
    public string DiscountType { get; set; } = "Percentage";
    public decimal DiscountValue { get; set; }
}

public class SaveBundleItemRequest
{
    public long SubcategoryId { get; set; }
    public int RequiredQty { get; set; }
}

public class SavePromotionRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DiscountType { get; set; } = "Percentage";
    public decimal DiscountValue { get; set; }
    public string Scope { get; set; } = "Global";
    public List<long> TargetCategoryIds { get; set; } = [];
    public List<long> TargetSubcategoryIds { get; set; } = [];
    public List<SaveVolumeTierRequest> VolumeTiers { get; set; } = [];
    public List<SaveBundleItemRequest> BundleItems { get; set; } = [];
    public decimal? MinOrderAmount { get; set; }
    public DateTime? StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsOneTimePerUser { get; set; }
}

public class SavePromoCodeRequest
{
    public string? Code { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string CardColor { get; set; } = "#FF6B9D";
    public string? Description { get; set; }
    public string DiscountType { get; set; } = "Percentage";
    public decimal DiscountValue { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public int? MaxUsages { get; set; }
    public bool IsOneTimePerUser { get; set; }
    public DateTime? StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public bool IsActive { get; set; } = true;
}

// ─── Public responses ──────────────────────────────────────────────────────────

public class PublicPromotionResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DiscountType { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public string Scope { get; set; } = string.Empty;
    public decimal? MinOrderAmount { get; set; }
    public DateTime? StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class PromoCodeCardResponse
{
    public long Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string CardColor { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DiscountType { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public DateTime? EndsAt { get; set; }
}

// ─── Checkout DTOs ─────────────────────────────────────────────────────────────

public class ActivatePromoCodeRequest
{
    public string Code { get; set; } = string.Empty;
}

public class CartItemForDiscount
{
    public long? ProductId { get; set; }
    public int Qty { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CalculateDiscountRequest
{
    public decimal OrderTotal { get; set; }
    public long? UserPromoCardId { get; set; }
    public List<CartItemForDiscount> Items { get; set; } = [];
}

public class CalculateDiscountResponse
{
    public decimal OriginalTotal { get; set; }
    public decimal PromotionDiscount { get; set; }
    public decimal PromoCodeDiscount { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal FinalTotal { get; set; }
    public PublicPromotionResponse? AppliedPromotion { get; set; }
    public PromoCodeCardResponse? AppliedPromoCode { get; set; }
}
