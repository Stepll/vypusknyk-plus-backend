using VypusknykPlus.Application.ValueObjects;

namespace VypusknykPlus.Application.DTOs.Admin;

public class AdminOrderItemResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public NamesData? NamesData { get; set; }
    public RibbonCustomization? RibbonCustomization { get; set; }
}

public class AdminOrderResponse
{
    public long Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public bool IsAnonymous { get; set; }
    public long? UserId { get; set; }
    public string? Email { get; set; }
    public string? Comment { get; set; }
    public string Payment { get; set; } = string.Empty;
    public AdminRecipientResponse Recipient { get; set; } = new();
    public AdminDeliveryResponse Delivery { get; set; } = new();
    public List<AdminOrderItemResponse> Items { get; set; } = [];
}

public class AdminRecipientResponse
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

public class AdminDeliveryResponse
{
    public string Method { get; set; } = string.Empty;
    public string MethodName { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Warehouse { get; set; }
    public string? PostalCode { get; set; }
}
