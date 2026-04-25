namespace VypusknykPlus.Application.DTOs.Admin;

public class DashboardResponse
{
    public DashboardRevenueBlock Revenue { get; set; } = new();
    public DashboardOrdersBlock Orders { get; set; } = new();
    public List<DashboardChartPoint> Chart { get; set; } = [];
    public DashboardDeliveriesBlock Deliveries { get; set; } = new();
    public DashboardDesignsBlock Designs { get; set; } = new();
    public List<DashboardTopCategoryBlock> TopProducts { get; set; } = [];
    public List<DashboardRecentOrder> RecentOrders { get; set; } = [];
}

public class DashboardRevenueBlock
{
    public decimal CurrentMonth { get; set; }
    public decimal PreviousMonth { get; set; }
    public double ChangePercent { get; set; }
    public double AvgProductionDays { get; set; }
}

public class DashboardOrdersBlock
{
    public List<DashboardStatusCount> StatusCounts { get; set; } = [];
    public int NewThisWeek { get; set; }
    public int Stuck { get; set; }
}

public class DashboardStatusCount
{
    public long StatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string StatusColor { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsFinal { get; set; }
    public int Count { get; set; }
}

public class DashboardRecentOrder
{
    public long Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string? ClientName { get; set; }
    public decimal Total { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string StatusColor { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class DashboardChartPoint
{
    public string Date { get; set; } = string.Empty;
    public int Orders { get; set; }
    public int Visits { get; set; }
}

public class DashboardDeliveriesBlock
{
    public List<DashboardAwaitingDelivery> Awaiting { get; set; } = [];
    public List<DashboardUpcomingDelivery> Upcoming { get; set; } = [];
}

public class DashboardAwaitingDelivery
{
    public long Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string? SupplierName { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TotalExpected { get; set; }
    public int TotalReceived { get; set; }
}

public class DashboardUpcomingDelivery
{
    public long Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public string? SupplierName { get; set; }
    public string ExpectedDate { get; set; } = string.Empty;
}

public class DashboardDesignsBlock
{
    public int SavedThisWeek { get; set; }
    public List<DashboardTopItem> TopColors { get; set; } = [];
    public List<DashboardTopItem> TopEmblems { get; set; } = [];
    public List<DashboardTopItem> TopFonts { get; set; } = [];
}

public class DashboardTopItem
{
    public string Key { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class DashboardTopCategoryBlock
{
    public string Category { get; set; } = string.Empty;
    public int TotalSold { get; set; }
    public List<DashboardTopProduct> Products { get; set; } = [];
}

public class DashboardTopProduct
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
