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

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<OrderItem> Items { get; set; } = [];
}
