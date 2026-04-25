namespace VypusknykPlus.Application.DTOs.Admin;

public class DashboardChartResponse
{
    public List<DashboardChartPoint2> Points { get; set; } = [];
}

public class DashboardChartPoint2
{
    public string Date { get; set; } = string.Empty;
    public int Orders { get; set; }
    public double Revenue { get; set; }
}

public class DashboardDistributionsResponse
{
    public List<DashboardDistributionItem> DeliveryMethods { get; set; } = [];
    public List<DashboardDistributionItem> Materials { get; set; } = [];
    public List<DashboardDistributionItem> Colors { get; set; } = [];
}

public class DashboardDistributionItem
{
    public string Key { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class DashboardTopItemsResponse
{
    public int ActiveCount { get; set; }
    public List<DashboardTopItemEntry> Items { get; set; } = [];
}

public class DashboardTopItemEntry
{
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
}

public class DashboardLowStockResponse
{
    public List<DashboardLowStockItem> Items { get; set; } = [];
}

public class DashboardLowStockItem
{
    public string Name { get; set; } = string.Empty;
    public string Material { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Stock { get; set; }
}

public class SalesByCategoryResponse
{
    public List<SalesCategoryEntry> Categories { get; set; } = [];
}

public class SalesCategoryEntry
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TotalSold { get; set; }
    public List<SalesProductEntry> TopProducts { get; set; } = [];
    public List<SalesSubcategoryEntry> Subcategories { get; set; } = [];
}

public class SalesSubcategoryEntry
{
    public long Id { get; set; }
    public long CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TotalSold { get; set; }
    public List<SalesProductEntry> TopProducts { get; set; } = [];
}

public class SalesProductEntry
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
