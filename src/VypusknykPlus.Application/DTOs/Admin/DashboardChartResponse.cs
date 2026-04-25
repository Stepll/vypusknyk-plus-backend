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
