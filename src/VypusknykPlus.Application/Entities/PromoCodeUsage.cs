namespace VypusknykPlus.Application.Entities;

public class PromoCodeUsage
{
    public long Id { get; set; }
    public long PromoCodeId { get; set; }
    public PromoCode PromoCode { get; set; } = null!;
    public long? UserId { get; set; }
    public User? User { get; set; }
    public long OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public decimal DiscountAmount { get; set; }
    public DateTime UsedAt { get; set; }
}
