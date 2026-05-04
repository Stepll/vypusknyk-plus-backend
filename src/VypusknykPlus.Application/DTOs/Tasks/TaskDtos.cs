namespace VypusknykPlus.Application.DTOs.Tasks;

public class AdminTaskResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string TaskType { get; set; } = string.Empty;
    public decimal TargetValue { get; set; }
    public long? TargetCategoryId { get; set; }
    public string? TargetCategoryName { get; set; }
    public long RewardPromoCodeId { get; set; }
    public string RewardPromoCodeDisplayName { get; set; } = string.Empty;
    public string RewardPromoCodeCardColor { get; set; } = string.Empty;
    public string RewardDiscountType { get; set; } = string.Empty;
    public decimal RewardDiscountValue { get; set; }
    public bool IsVisibleToGuests { get; set; }
    public DateTime? EndsAt { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public int CompletionsCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SaveTaskRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string TaskType { get; set; } = string.Empty;
    public decimal TargetValue { get; set; }
    public long? TargetCategoryId { get; set; }
    public long RewardPromoCodeId { get; set; }
    public bool IsVisibleToGuests { get; set; }
    public DateTime? EndsAt { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}

public class PublicTaskResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string TaskType { get; set; } = string.Empty;
    public decimal TargetValue { get; set; }
    public long? TargetCategoryId { get; set; }
    public string? TargetCategoryName { get; set; }
    public bool IsVisibleToGuests { get; set; }
    public DateTime? EndsAt { get; set; }
    public string RewardDisplayName { get; set; } = string.Empty;
    public string RewardCardColor { get; set; } = string.Empty;
    public string RewardDiscountType { get; set; } = string.Empty;
    public decimal RewardDiscountValue { get; set; }
    public decimal? UserProgress { get; set; }
    public bool IsCompleted { get; set; }
}
