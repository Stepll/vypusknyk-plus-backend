namespace VypusknykPlus.Application.DTOs.Admin;

public class SupplierResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? TaxId { get; set; }
    public string? Address { get; set; }
    public string? Notes { get; set; }
}

public class SaveSupplierRequest
{
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? TaxId { get; set; }
    public string? Address { get; set; }
    public string? Notes { get; set; }
}

public class DeliverySummary
{
    public long Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public long? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public string ExpectedDate { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Note { get; set; }
    public int ItemCount { get; set; }
    public int TotalExpectedQty { get; set; }
    public int TotalReceivedQty { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
}

public class ReceiveTransactionInfo
{
    public long Id { get; set; }
    public int Quantity { get; set; }
    public string Date { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}

public class DeliveryItemResponse
{
    public long Id { get; set; }
    public long DeliveryId { get; set; }
    public long ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SubcategoryName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public bool HasColor { get; set; }
    public bool HasMaterial { get; set; }
    public string Material { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int ExpectedQty { get; set; }
    public int ReceivedQty { get; set; }
    public string? ReceivedAt { get; set; }
    public List<ReceiveTransactionInfo> ReceiveHistory { get; set; } = [];
}

public class DeliveryDetail
{
    public long Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public long? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public string ExpectedDate { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Note { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
    public List<DeliveryItemResponse> Items { get; set; } = [];
}

public class CreateDeliveryItemRequest
{
    public long ProductId { get; set; }
    public string Material { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int ExpectedQty { get; set; }
}

public class CreateDeliveryRequest
{
    public long? SupplierId { get; set; }
    public string ExpectedDate { get; set; } = string.Empty;
    public string? Note { get; set; }
    public List<CreateDeliveryItemRequest> Items { get; set; } = [];
}

public class ReceiveDeliveryItemRequest
{
    public int Quantity { get; set; }
    public string Date { get; set; } = string.Empty;
    public string? Note { get; set; }
}

public class ReceiveAllRequest
{
    public string Date { get; set; } = string.Empty;
}

public class DeliveryQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public long? SupplierId { get; set; }
    public string? Status { get; set; }
    public string? Search { get; set; }
}
