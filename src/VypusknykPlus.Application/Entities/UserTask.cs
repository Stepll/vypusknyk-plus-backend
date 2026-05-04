namespace VypusknykPlus.Application.Entities;

public enum TaskType
{
    Registration = 0,
    FirstOrder = 1,
    ProfileComplete = 2,
    OrdersCount = 3,
    TotalSpent = 4,
    OrderAmount = 5,
    CategoryOrders = 6,
    AccountActivation = 7,
}

public class UserTask : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskType TaskType { get; set; }
    public decimal TargetValue { get; set; }
    public long? TargetCategoryId { get; set; }
    public ProductCategory? TargetCategory { get; set; }
    public long RewardPromoCodeId { get; set; }
    public PromoCode RewardPromoCode { get; set; } = null!;
    public bool IsVisibleToGuests { get; set; }
    public DateTime? EndsAt { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }

    public ICollection<UserTaskProgress> Progresses { get; set; } = [];
}
