using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/orders")]
public class AdminOrdersController : ControllerBase
{
    private readonly IAdminService _admin;

    public AdminOrdersController(IAdminService admin) => _admin = admin;

    [HttpGet]
    public async Task<ActionResult<PagedResponse<AdminOrderResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null)
    {
        return Ok(await _admin.GetOrdersAsync(page, pageSize, status));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<AdminOrderResponse>> GetById(long id)
    {
        var order = await _admin.GetOrderAsync(id);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPatch("{id:long}/status")]
    public async Task<IActionResult> UpdateStatus(long id, [FromBody] UpdateOrderStatusRequest request)
    {
        await _admin.UpdateOrderStatusAsync(id, request.Status);
        return NoContent();
    }
}
