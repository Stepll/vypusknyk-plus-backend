using VypusknykPlus.Application.DTOs.Admin;

namespace VypusknykPlus.Application.Services;

public interface IDashboardService
{
    Task<DashboardResponse> GetAsync();
    Task<DashboardStatsResponse> GetStatsAsync(string period);
    Task<DashboardChartResponse> GetChartAsync(string period);
    Task<DashboardDistributionsResponse> GetDistributionsAsync(string period);
    Task<DashboardTopItemsResponse> GetTopItemsAsync(string period, string metric);
    Task<DashboardLowStockResponse> GetLowStockAsync();
    Task<DashboardDesignsBlock> GetDesignsAsync(string period);
}
