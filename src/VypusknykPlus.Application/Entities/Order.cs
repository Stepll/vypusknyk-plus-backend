using VypusknykPlus.Application.ValueObjects;

namespace VypusknykPlus.Application.Entities;

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public long StatusId { get; set; }
    public OrderStatus OrderStatus { get; set; } = null!;
    public decimal Total { get; set; }

    public DeliveryInfo Delivery { get; set; } = new();
    public RecipientInfo Recipient { get; set; } = new();

    public long DeliveryMethodId { get; set; }
    public DeliveryMethod DeliveryMethod { get; set; } = null!;

    public long PaymentMethodId { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = null!;

    public string? Email { get; set; }
    public string? Comment { get; set; }

    public bool IsAnonymous { get; set; }
    public string? GuestToken { get; set; }

    public long? UserId { get; set; }
    public User? User { get; set; }

    public ICollection<OrderItem> Items { get; set; } = [];
}
