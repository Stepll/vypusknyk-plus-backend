namespace VypusknykPlus.Application.DTOs.Orders;

public class OrderItemResponse
{
    public string Name { get; set; } = string.Empty;
    public int Qty { get; set; }
    public decimal Price { get; set; }
}

public class OrderDeliveryResponse
{
    public string Method { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Warehouse { get; set; }
    public string? PostalCode { get; set; }
}

public class OrderRecipientResponse
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

public class OrderResponse
{
    public long Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<OrderItemResponse> Items { get; set; } = [];
    public decimal Total { get; set; }
    public OrderDeliveryResponse Delivery { get; set; } = new();
    public OrderRecipientResponse Recipient { get; set; } = new();
    public string Payment { get; set; } = string.Empty;
    public string? Comment { get; set; }
}
