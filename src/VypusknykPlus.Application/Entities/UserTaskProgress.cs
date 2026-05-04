namespace VypusknykPlus.Application.Entities;

public class UserTaskProgress
{
    public long Id { get; set; }
    public long TaskId { get; set; }
    public UserTask Task { get; set; } = null!;
    public long UserId { get; set; }
    public User User { get; set; } = null!;
    public decimal Progress { get; set; }
    public DateTime? CompletedAt { get; set; }
    public long? AwardedCardId { get; set; }
    public UserPromoCodeCard? AwardedCard { get; set; }
}
