namespace VypusknykPlus.Application.DTOs.Promotions;

// ─── Admin responses ───────────────────────────────────────────────────────────

public class AdminPromotionResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DiscountType { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public string Scope { get; set; } = string.Empty;
    public long? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public long? SubcategoryId { get; set; }
    public string? SubcategoryName { get; set; }
    public long? ProductId { get; set; }
    public string? ProductName { get; set; }
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
    public string Code { get; set; } = string.Empty;
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

public class SavePromotionRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DiscountType { get; set; } = "Percentage";
    public decimal DiscountValue { get; set; }
    public string Scope { get; set; } = "Global";
    public long? CategoryId { get; set; }
    public long? SubcategoryId { get; set; }
    public long? ProductId { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public DateTime? StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsOneTimePerUser { get; set; }
}

public class SavePromoCodeRequest
{
    public string Code { get; set; } = string.Empty;
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

public class CalculateDiscountRequest
{
    public decimal OrderTotal { get; set; }
    public long? UserPromoCardId { get; set; }
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
