namespace VypusknykPlus.Application.DTOs.Admin;

public class DashboardStatsResponse
{
    public DashboardStatMetric Revenue { get; set; } = new();
    public DashboardStatMetric OrdersCount { get; set; } = new();
    public DashboardStatMetric AvgCheck { get; set; } = new();
}

public class DashboardStatMetric
{
    public double Current { get; set; }
    public double Previous { get; set; }
    public double ChangePercent { get; set; }
    public List<double> Sparkline { get; set; } = [];
}
