using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/dashboard")]
public class AdminDashboardController : ControllerBase
{
    private readonly IDashboardService _dashboard;

    public AdminDashboardController(IDashboardService dashboard) => _dashboard = dashboard;

    [HttpGet]
    public async Task<ActionResult<DashboardResponse>> Get()
    {
        return Ok(await _dashboard.GetAsync());
    }

    [HttpGet("stats")]
    public async Task<ActionResult<DashboardStatsResponse>> GetStats([FromQuery] string period = "month")
    {
        return Ok(await _dashboard.GetStatsAsync(period));
    }

    [HttpGet("chart")]
    public async Task<ActionResult<DashboardChartResponse>> GetChart([FromQuery] string period = "month")
    {
        return Ok(await _dashboard.GetChartAsync(period));
    }

    [HttpGet("distributions")]
    public async Task<ActionResult<DashboardDistributionsResponse>> GetDistributions([FromQuery] string period = "month")
    {
        return Ok(await _dashboard.GetDistributionsAsync(period));
    }
}
