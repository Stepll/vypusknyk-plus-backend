using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/order-statuses")]
public class AdminOrderStatusesController : ControllerBase
{
    private readonly AppDbContext _db;

    public AdminOrderStatusesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<OrderStatusResponse>>> GetAll()
    {
        var statuses = await _db.OrderStatuses
            .AsNoTracking()
            .OrderBy(s => s.SortOrder)
            .ToListAsync();

        return Ok(statuses.Select(Map).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<OrderStatusResponse>> Create([FromBody] SaveOrderStatusRequest request)
    {
        var status = new OrderStatus
        {
            Name = request.Name,
            Color = request.Color,
            SortOrder = request.SortOrder,
            IsFinal = request.IsFinal,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _db.OrderStatuses.Add(status);
        await _db.SaveChangesAsync();
        return Ok(Map(status));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<OrderStatusResponse>> Update(long id, [FromBody] SaveOrderStatusRequest request)
    {
        var status = await _db.OrderStatuses.FirstOrDefaultAsync(s => s.Id == id);
        if (status is null) return NotFound();

        status.Name = request.Name;
        status.Color = request.Color;
        status.SortOrder = request.SortOrder;
        status.IsFinal = request.IsFinal;
        status.IsActive = request.IsActive;
        status.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(Map(status));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var status = await _db.OrderStatuses.FirstOrDefaultAsync(s => s.Id == id);
        if (status is null) return NotFound();

        status.IsDeleted = true;
        status.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static OrderStatusResponse Map(OrderStatus s) => new()
    {
        Id = s.Id,
        Name = s.Name,
        Color = s.Color,
        SortOrder = s.SortOrder,
        IsFinal = s.IsFinal,
        IsActive = s.IsActive,
    };
}
