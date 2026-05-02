using VypusknykPlus.Application.ValueObjects;

namespace VypusknykPlus.Application.DTOs.Orders;

public class CreateOrderItemRequest
{
    public int? ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Qty { get; set; }
    public decimal Price { get; set; }
    public NamesData? NamesData { get; set; }
    public RibbonCustomization? RibbonCustomization { get; set; }
}

public class CreateOrderDeliveryRequest
{
    public string Method { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Warehouse { get; set; }
    public string? PostalCode { get; set; }
}

public class CreateOrderRecipientRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

public class CreateOrderRequest
{
    public List<CreateOrderItemRequest> Items { get; set; } = [];
    public CreateOrderDeliveryRequest Delivery { get; set; } = new();
    public CreateOrderRecipientRequest Recipient { get; set; } = new();
    public string Payment { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Comment { get; set; }
    public string? GuestToken { get; set; }
    public long? UserPromoCardId { get; set; }
}
