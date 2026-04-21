namespace VypusknykPlus.Application.DTOs.Admin;

public class StockCategoryResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
}

public class StockSubcategoryResponse
{
    public long Id { get; set; }
    public long CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
}

public class StockProductSummary
{
    public long Id { get; set; }
    public long SubcategoryId { get; set; }
    public string SubcategoryName { get; set; } = string.Empty;
    public long CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool HasColor { get; set; }
    public bool HasMaterial { get; set; }
    public int TotalStock { get; set; }
    public int VariantCount { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class StockVariantResponse
{
    public long Id { get; set; }
    public string Material { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
}

public class StockTransactionResponse
{
    public long Id { get; set; }
    public long VariantId { get; set; }
    public long? DeliveryItemId { get; set; }
    public string Material { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Date { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}

public class StockProductDetail
{
    public long Id { get; set; }
    public long SubcategoryId { get; set; }
    public string SubcategoryName { get; set; } = string.Empty;
    public long CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool HasColor { get; set; }
    public bool HasMaterial { get; set; }
    public List<StockVariantResponse> Variants { get; set; } = [];
    public List<StockTransactionResponse> Transactions { get; set; } = [];
}

public class WarehouseStatsResponse
{
    public int TotalStock { get; set; }
    public int OutOfStockCount { get; set; }
    public int LowStockCount { get; set; }
    public int CategoryCount { get; set; }
    public int ProductCount { get; set; }
}

public class CreateStockTransactionRequest
{
    public long ProductId { get; set; }
    public string Material { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Date { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
}

public class CreateStockProductRequest
{
    public long SubcategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool HasColor { get; set; }
    public bool HasMaterial { get; set; }
}

public class WarehouseProductsQuery
{
    public long? CategoryId { get; set; }
    public long? SubcategoryId { get; set; }
    public string? Material { get; set; }
    public string? Status { get; set; }
    public string? Search { get; set; }
    public string? Color { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
