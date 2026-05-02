namespace VypusknykPlus.Application.Entities;

public class UserPromoCodeCard
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public User User { get; set; } = null!;
    public long PromoCodeId { get; set; }
    public PromoCode PromoCode { get; set; } = null!;
    public DateTime ActivatedAt { get; set; }
}
