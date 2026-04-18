using VypusknykPlus.Application.ValueObjects;

namespace VypusknykPlus.Application.Entities;

public enum OrderStatus
{
    Accepted,
    Production,
    Shipped,
    Delivered
}

public enum PaymentMethod
{
    Cod,
    Online
}

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public decimal Total { get; set; }

    public DeliveryInfo Delivery { get; set; } = new();
    public RecipientInfo Recipient { get; set; } = new();

    public PaymentMethod Payment { get; set; }
    public string? Email { get; set; }
    public string? Comment { get; set; }

    public bool IsAnonymous { get; set; }
    public string? GuestToken { get; set; }

    public long? UserId { get; set; }
    public User? User { get; set; }

    public ICollection<OrderItem> Items { get; set; } = [];
}
