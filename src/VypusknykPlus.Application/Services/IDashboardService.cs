using VypusknykPlus.Application.DTOs.Admin;

namespace VypusknykPlus.Application.Services;

public interface IDashboardService
{
    Task<DashboardResponse> GetAsync();
    Task<DashboardStatsResponse> GetStatsAsync(string period);
}
